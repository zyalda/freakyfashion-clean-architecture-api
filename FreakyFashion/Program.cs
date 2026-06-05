using ApplicationLayer;
using DomainLayer;
using Microsoft.IdentityModel.Tokens;
using RepositoriesDependencyInjectionProject;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFashionDataBaseContext(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddRepositoriesInjection();
builder.Services.AddApplicationCore();
builder.Services.AddAuthenticationJwtBearer();
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod().WithExposedHeaders("Content-Disposition"); // Important!;
    });
});

builder.Services.AddAuthentication("Bearer")
.AddJwtBearer(options =>
{
    //Try to fix Bearer error="invalid_token", error_description="The audience by hardcoding here.
    // Direct fallback check for Azure Environment Variables
    var issuer = builder.Configuration["Jwt:Issuer"]
                 ?? Environment.GetEnvironmentVariable("Jwt__Issuer");

    var audience = builder.Configuration["Jwt:Audience"]
                   ?? Environment.GetEnvironmentVariable("Jwt__Audience");

    var secretKey = builder.Configuration["Jwt:Key"]
                    ?? Environment.GetEnvironmentVariable("Jwt__Key");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),

        // 2. Validate using the forced variables
        ValidateIssuer = true,
        ValidIssuer = issuer,

        ValidateAudience = true,
        ValidAudience = audience,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseRouting();


if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    // Skapar JSON-filen på /openapi/v1.json
    app.MapOpenApi();

    string? apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

    // Startar Scalar UI på /scalar/v1
    app.MapScalarApiReference("/scalar/v1", (options, context) =>
    {
        if (!string.IsNullOrEmpty(apiBaseUrl))
        {
            options.AddServer(apiBaseUrl);
        }
    });
}

app.UseHttpsRedirection();

//Detta måste sättas upp efter routing
app.UseAuthentication();
app.UseAuthorization();

//app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.MapControllers();

app.Run();
