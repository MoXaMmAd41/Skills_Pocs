using EmployeeManagement.Application.Common.Caching;
using EmployeeManagement.Application.Features.Auth;
using EmployeeManagement.Application.Features.Dashboard;
using EmployeeManagement.Application.Features.Departments;
using EmployeeManagement.Application.Features.Employees;
using EmployeeManagement.Domain.Common;
using EmployeeManagement.Domain.Departments;
using EmployeeManagement.Domain.Employees;
using EmployeeManagement.Infrastructure.Caching;
using EmployeeManagement.Infrastructure.Identity;
using EmployeeManagement.Infrastructure.Persistence;
using EmployeeManagement.Infrastructure.Persistence.Repositories;
using EmployeeManagement.Infrastructure.Queries;
using EmployeeManagement.Infrastructure.Search;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        return services
            .AddPersistence(configuration)
            .AddQueries(configuration)
            .AddCaching(configuration)
            .AddIdentityAndJwt(configuration);
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>().Bind(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(GetConnectionString(configuration)));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        return services;
    }

    private static IServiceCollection AddQueries(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new SqlConnectionFactory(GetConnectionString(configuration)));
        services.AddScoped<IEmployeeQueries, EmployeeQueries>();
        services.AddScoped<IDepartmentQueries, DepartmentQueries>();
        services.AddScoped<IDashboardQueries, DashboardQueries>();
        services.AddSingleton<IEmployeeSearchIndex, TrieEmployeeSearchIndex>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        string? redisConnection = configuration.GetConnectionString("Redis");

        // Without Redis (e.g. running locally with no containers) fall back to an in-process cache.
        if (string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "EmployeeManagement:";
            });
        }

        services.AddSingleton<ICacheService, DistributedCacheService>();

        return services;
    }

    private static IServiceCollection AddIdentityAndJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwt) =>
            {
                bearer.MapInboundClaims = false;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Value.Issuer,
                    ValidAudience = jwt.Value.Audience,
                    IssuerSigningKey = JwtTokenGenerator.CreateSigningKey(jwt.Value),
                    NameClaimType = JwtTokenGenerator.NameClaim,
                    RoleClaimType = JwtTokenGenerator.RoleClaim,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddSingleton<JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    private static string GetConnectionString(IConfiguration configuration) =>
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
}
