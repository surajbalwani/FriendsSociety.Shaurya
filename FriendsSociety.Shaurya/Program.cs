using Azure.Identity;
using FriendsSociety.Shaurya.Configuration;
using FriendsSociety.Shaurya.Data;
using FriendsSociety.Shaurya.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;
using Microsoft.Extensions.Logging;

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

// Configure Swagger to support JWT Bearer authentication
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\r\nExample: 'Bearer eyJhbGciOi...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});

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

// When Identity uses cookie authentication for some flows it will redirect API calls to a login page.
// For API endpoints we want JSON 401/403 responses with helpful links instead of redirects.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new
            {
                error = "Unauthenticated",
                message = "Authentication required to access this resource.",
                loginUrl = "/api/account/login",
                registerUrl = "/api/account/register"
            });
            return context.Response.WriteAsync(payload);
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new
            {
                error = "Forbidden",
                message = "You do not have permission to access this resource.",
                contact = "/api/account/register"
            });
            return context.Response.WriteAsync(payload);
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

// Register JWT authentication. If you have real JWT settings in configuration (Jwt:Key, Jwt:Issuer, Jwt:Audience)
// replace the fallback below to enable proper validation.
var jwtSecret = builder.Configuration["JwtSettings:Secret"] ?? builder.Configuration["Jwt:Key"] ?? builder.Configuration["JwtSettings:Key"];
if (!string.IsNullOrEmpty(jwtSecret))
{
    var keyBytes = Encoding.UTF8.GetBytes(jwtSecret);
    var symmetricKey = new SymmetricSecurityKey(keyBytes);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = symmetricKey,
            ValidateIssuer = !string.IsNullOrEmpty(builder.Configuration["Jwt:Issuer"]) || !string.IsNullOrEmpty(builder.Configuration["JwtSettings:Issuer"]),
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? builder.Configuration["Jwt:Issuer"],
            ValidateAudience = !string.IsNullOrEmpty(builder.Configuration["Jwt:Audience"]) || !string.IsNullOrEmpty(builder.Configuration["JwtSettings:Audience"]),
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? builder.Configuration["Jwt:Audience"],

            // Fallback resolver: some tokens may not include 'kid' header. Ensure symmetric key is available for validation.
            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                // Always return our symmetric key so tokens signed with this secret validate even when 'kid' is missing.
                return new[] { symmetricKey };
            }
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var header = ctx.Request.Headers["Authorization"].FirstOrDefault();
                // optional: log header presence
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                // logs the exception that caused authentication to fail
                var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JwtAuth");
                logger.LogWarning(ctx.Exception, "JWT authentication failed: {Message}", ctx.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JwtAuth");
                logger.LogInformation("JWT token validated for {sub}", ctx.Principal?.Identity?.Name ?? ctx.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value);
                return Task.CompletedTask;
            }
        };
    });
}
else
{
    // Development fallback: register authentication but skip strict token validation so Swagger UI can be used.
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = false,
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
}

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

// Enable authentication middleware so Swagger 'Authorize' will send header to endpoints
app.UseAuthentication();

// Middleware to capture roles of authenticated user and attach to HttpContext.Items["UserRoles"]
app.Use(async (context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        var roles = context.User.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
            .Select(c => c.Value)
            .ToList();

        context.Items["UserRoles"] = roles;

        var logger = context.RequestServices.GetService<ILogger<Program>>();
        logger?.LogDebug("Captured roles for user {User}: {Roles}", context.User.Identity?.Name, string.Join(',', roles));
    }

    await next();
});

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
