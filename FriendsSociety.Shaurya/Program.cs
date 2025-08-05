using Azure.Identity;
using FriendsSociety.Shaurya.Configuration;
using FriendsSociety.Shaurya.Data;
using FriendsSociety.Shaurya.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Add Azure Key Vault only in production
if (builder.Environment.IsProduction())
{
    var keyVaultName = "shaurya-kv"; // your vault name
    var kvUri = $"https://{keyVaultName}.vault.azure.net/";
    builder.Configuration.AddAzureKeyVault(new Uri(kvUri), new DefaultAzureCredential());
}
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var originsConfig = builder.Configuration.GetValue<string>("allowedOrigins");
var allowedOrigins = originsConfig?.Split(",") ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("angularApplication", builder2 =>
    {
        builder2.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // Add this line if you need to send cookies or authentication headers
    });
});
builder.Services.AddDbContext<DataContext>(options => {
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null // Let EF handle transient errors
        )
    );
});

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
var app = builder.Build();

// Apply database migrations with error handling
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        Log.Information("Applying database migrations...");
        db.Database.Migrate(); // applies any pending migrations
        Log.Information("Database migrations applied successfully.");
    }
}
catch (Exception ex)
{
    Log.Error(ex, "An error occurred while applying database migrations.");
    // Don't throw in production to allow the app to start even if migrations fail
    if (!app.Environment.IsProduction())
    {
        throw;
    }
}

// Seed demo data with error handling (only if enabled)
var shouldSeedData = builder.Configuration.GetValue<bool>("DatabaseSettings:SeedDemoData");
if (shouldSeedData)
{
    try
    {
        Log.Information("Starting demo data seeding...");
        await ModelSeeder.SeedDemoDataAsync(app.Services);
        Log.Information("Demo data seeding completed successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while seeding demo data.");
        // Don't throw to allow the app to start even if seeding fails
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}
app.UseCors("angularApplication");
app.UseHttpsRedirection();


app.UseAuthorization();

// Default route
app.MapGet("/", () => "Welcome to the API!");

app.MapControllers();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
