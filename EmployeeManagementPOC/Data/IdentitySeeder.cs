using EmployeeManagementPOC.Constants;
using EmployeeManagementPOC.Models;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagementPOC.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider services)
    {
        RoleManager<IdentityRole> roleManager =
            services.GetRequiredService<
                RoleManager<IdentityRole>>();

        UserManager<ApplicationUser> userManager =
            services.GetRequiredService<
                UserManager<ApplicationUser>>();

        string[] roles =
        [
            AppRoles.Admin,
            AppRoles.HR,
            AppRoles.Manager,
            AppRoles.Employee
        ];

        foreach (string role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                IdentityResult roleResult =
                    await roleManager.CreateAsync(
                        new IdentityRole(role));

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create role '{role}': " +
                        string.Join(
                            ", ",
                            roleResult.Errors.Select(
                                e => e.Description)));
                }
            }
        }

        await CreateUserAsync(
            userManager,
            email: "admin.user@poc.local",
            password: "Admin@12345",
            fullName: "System Administrator",
            role: AppRoles.Admin);

        await CreateUserAsync(
            userManager,
            email: "hr.user@poc.local",
            password: "Hr@12345",
            fullName: "Human Resources",
            role: AppRoles.HR);

        await CreateUserAsync(
            userManager,
            email: "manager.user@poc.local",
            password: "Manager@12345",
            fullName: "Department Manager",
            role: AppRoles.Manager);

        await CreateUserAsync(
            userManager,
            email: "employee.user@poc.local",
            password: "Employee@12345",
            fullName: "Normal Employee",
            role: AppRoles.Employee);
    }

    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string fullName,
        string role)
    {
        ApplicationUser? existingUser =
            await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            if (!await userManager.IsInRoleAsync(
                    existingUser,
                    role))
            {
                IdentityResult roleResult =
                    await userManager.AddToRoleAsync(
                        existingUser,
                        role);

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to assign role '{role}' " +
                        $"to '{email}': " +
                        string.Join(
                            ", ",
                            roleResult.Errors.Select(
                                e => e.Description)));
                }
            }

            return;
        }

        ApplicationUser user = new()
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName
        };

        IdentityResult createResult =
            await userManager.CreateAsync(
                user,
                password);

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create user '{email}': " +
                string.Join(
                    ", ",
                    createResult.Errors.Select(
                        e => e.Description)));
        }

        IdentityResult addRoleResult =
            await userManager.AddToRoleAsync(
                user,
                role);

        if (!addRoleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to assign role '{role}' " +
                $"to '{email}': " +
                string.Join(
                    ", ",
                    addRoleResult.Errors.Select(
                        e => e.Description)));
        }
    }
}