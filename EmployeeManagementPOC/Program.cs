using EmployeeManagementPOC.Constants;
using EmployeeManagementPOC.Data;
using EmployeeManagementPOC.Interfaces;
using EmployeeManagementPOC.Middleware;
using EmployeeManagementPOC.Models;
using EmployeeManagementPOC.Repositories;
using EmployeeManagementPOC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

builder.Services.AddSingleton<DapperDbContext>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:Connection"];

    options.InstanceName =
        builder.Configuration["Redis:InstanceName"];
});

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(10);

        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";

    options.AccessDeniedPath = "/Account/AccessDenied";

    options.ExpireTimeSpan =
        TimeSpan.FromHours(8);

    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DashboardAccess", policy =>
    {
        policy.RequireRole(
            AppRoles.Admin,
            AppRoles.HR,
            AppRoles.Manager,
            AppRoles.Employee);
    });

    options.AddPolicy("EmployeeView", policy =>
    {
        policy.RequireRole(
            AppRoles.Admin,
            AppRoles.HR,
            AppRoles.Manager,
            AppRoles.Employee);
    });

    options.AddPolicy("EmployeeManage", policy =>
    {
        policy.RequireRole(
            AppRoles.Admin,
            AppRoles.HR);
    });

    options.AddPolicy("DepartmentView", policy =>
    {
        policy.RequireRole(
            AppRoles.Admin,
            AppRoles.HR,
            AppRoles.Manager);
    });

    options.AddPolicy("DepartmentManage", policy =>
    {
        policy.RequireRole(
            AppRoles.Admin,
            AppRoles.HR);
    });
});

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ReportService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

using (IServiceScope scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(
        scope.ServiceProvider);
}

app.Run();