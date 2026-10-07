using Asp.Versioning;

namespace EmployeeManagement.Api.Versioning;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddApiVersioningWithExplorer(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = ApiVersions.Default;
                options.AssumeDefaultVersionWhenUnspecified = true;

                // Emits api-supported-versions / api-deprecated-versions response headers.
                options.ReportApiVersions = true;

                // The URL segment is canonical; the header lets clients pin a version without changing routes.
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
