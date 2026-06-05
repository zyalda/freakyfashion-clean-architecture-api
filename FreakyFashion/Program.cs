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

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//Step 1
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseRouting();


if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    // 2. Skapar JSON-filen på /openapi/v1.json
    app.MapOpenApi();

    // 3. Startar Scalar UI på /scalar/v1
    app.MapScalarApiReference("/scalar/v1", (options, context) =>
    {
        options.AddServer("https://webapp-freakyfashion.azurewebsites.net");
    });
}

//app.MapScalarApiReference();   // Step 2
app.UseHttpsRedirection();

//Detta måste sättas upp efter routing
app.UseAuthentication();
app.UseAuthorization();

//app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.MapControllers();

app.Run();
