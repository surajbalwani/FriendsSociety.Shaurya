using FriendsSociety.Shaurya.Configuration;
using FriendsSociety.Shaurya.Data;
using FriendsSociety.Shaurya.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
var app = builder.Build();

// Seed all demo data at startup (service-based approach)
if (builder.Configuration.GetValue<bool>("DatabaseSettings:SeedDemoData"))
{
    await ModelSeeder.SeedDemoDataAsync(app.Services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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
