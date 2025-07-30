using Azure.Identity;
using FriendsSociety.Shaurya.Configuration;
using FriendsSociety.Shaurya.Data;
using FriendsSociety.Shaurya.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate(); // applies any pending migrations
}

if (builder.Configuration.GetValue<bool>("DatabaseSettings:SeedDemoData"))
{
    await ModelSeeder.SeedDemoDataAsync(app.Services);
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

app.Run();
