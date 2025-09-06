using CardManagement.Data;
using MerchantApp.Service.Extensions; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add controllers to the DI container
builder.Services.AddControllers();

// Enable API explorer for Swagger
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger and JWT authentication in Swagger UI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MerchantApp API",
        Version = "v1"
    });

    // Define Bearer token security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT Token. Example: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Require Bearer token globally for endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    });
});

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add in-memory caching
builder.Services.AddMemoryCache();

// Configure JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("JWT key not found in appsettings.json!");

// Add authentication with JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,              // Not validating issuer
            ValidateAudience = false,            // Not validating audience
            ValidateLifetime = true,             // Validate token expiration
            ValidateIssuerSigningKey = true,     // Validate the signing key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Add authorization middleware
builder.Services.AddAuthorization();

// Register application services (DI for all custom services)
builder.Services.AddMerchantAppServices(); 
// This calls your extension method which registers services like:
// ICardService, IProductService, ITransactionService, IQrService, etc.

// Build the app
var app = builder.Build();

// Enable Swagger in development
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MerchantApp API v1");
    c.RoutePrefix = "swagger"; // Swagger UI available at /swagger
});

// Middleware pipeline
app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseAuthentication();   // Enable authentication middleware
app.UseAuthorization();    // Enable authorization middleware

// Map controller routes
app.MapControllers();

// Run the application
app.Run();
