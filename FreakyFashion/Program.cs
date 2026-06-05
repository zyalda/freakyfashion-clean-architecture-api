using ApplicationLayer;
using DomainLayer;
using RepositoriesDependencyInjectionProject;
using Scalar.AspNetCore;

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
