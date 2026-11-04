using EmployeeManagement.Application.Common.Events;
using EmployeeManagement.Application.Features.Dashboard;
using EmployeeManagement.Application.Features.Departments;
using EmployeeManagement.Application.Features.Employees;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        services.AddDomainEventHandlers();

        return services;
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        IEnumerable<(Type Service, Type Implementation)> registrations =
            from type in typeof(DependencyInjection).Assembly.GetTypes()
            where type is { IsClass: true, IsAbstract: false }
            from contract in type.GetInterfaces()
            where contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
            select (contract, type);

        foreach ((Type service, Type implementation) in registrations)
        {
            services.AddScoped(service, implementation);
        }
    }
}
