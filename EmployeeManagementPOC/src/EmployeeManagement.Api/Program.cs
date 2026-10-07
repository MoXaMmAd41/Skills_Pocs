using System.Text.Json.Serialization;
using EmployeeManagement.Api.Authorization;
using EmployeeManagement.Api.ErrorHandling;
using EmployeeManagement.Api.Filters;
using EmployeeManagement.Api.OpenApi;
using EmployeeManagement.Api.Versioning;
using EmployeeManagement.Application;
using EmployeeManagement.Infrastructure;
using EmployeeManagement.Infrastructure.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiErrorHandling()
    .AddApiVersioningWithExplorer()
    .AddAuthorizationPolicies()
    .AddVersionedSwagger()
    .AddHealthChecks();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services
    .AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

WebApplication app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// TLS is terminated by the reverse proxy / ingress in front of the container.
if (app.Environment.IsDevelopment())
{
    app.UseVersionedSwaggerUi();
}

// Serves the compiled Angular app (copied into wwwroot by the Docker build).
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Unknown API routes must stay 404s; everything else is a client-side route handled by Angular.
app.MapFallback("/api/{**path}", () => Results.Problem(statusCode: StatusCodes.Status404NotFound));
app.MapFallbackToFile("index.html");

await app.Services.InitializeDatabaseAsync();

await app.RunAsync();
