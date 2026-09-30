using EmployeeManagement.Application.Common.Security;
using EmployeeManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    private sealed record DemoUser(string Email, string Password, string FullName, string Role);

    private static readonly DemoUser[] DemoUsers =
    [
        new("admin.user@poc.local", "Admin@12345", "System Administrator", Roles.Admin),
        new("hr.user@poc.local", "Hr@12345", "Human Resources", Roles.HR),
        new("manager.user@poc.local", "Manager@12345", "Department Manager", Roles.Manager),
        new("employee.user@poc.local", "Employee@12345", "Normal Employee", Roles.Employee)
    ];

    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        IServiceProvider provider = scope.ServiceProvider;

        DatabaseOptions options = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        ILogger logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DatabaseInitializer));

        if (options.ApplyMigrationsOnStartup)
        {
            logger.LogInformation("Applying database migrations.");
            await provider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
        }

        if (options.SeedDemoUsers)
        {
            logger.LogInformation("Seeding roles and demo users.");
            await SeedRolesAsync(provider.GetRequiredService<RoleManager<IdentityRole>>());
            await SeedDemoUsersAsync(provider.GetRequiredService<UserManager<ApplicationUser>>());
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (string role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                EnsureSucceeded(await roleManager.CreateAsync(new IdentityRole(role)), $"create role '{role}'");
            }
        }
    }

    private static async Task SeedDemoUsersAsync(UserManager<ApplicationUser> userManager)
    {
        foreach (DemoUser demo in DemoUsers)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(demo.Email);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = demo.Email,
                    Email = demo.Email,
                    EmailConfirmed = true,
                    FullName = demo.FullName
                };

                EnsureSucceeded(await userManager.CreateAsync(user, demo.Password), $"create user '{demo.Email}'");
            }

            if (!await userManager.IsInRoleAsync(user, demo.Role))
            {
                EnsureSucceeded(
                    await userManager.AddToRoleAsync(user, demo.Role),
                    $"assign role '{demo.Role}' to '{demo.Email}'");
            }
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to {operation}: {errors}");
        }
    }
}
