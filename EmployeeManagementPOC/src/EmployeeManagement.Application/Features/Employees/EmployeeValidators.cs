using EmployeeManagement.Domain.Employees;
using FluentValidation;

namespace EmployeeManagement.Application.Features.Employees;

internal sealed class EmployeeRequestValidator : AbstractValidator<EmployeeRequest>
{
    public EmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(Employee.NameMaxLength);

        RuleFor(x => x.LastName).NotEmpty().MaximumLength(Employee.NameMaxLength);

        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(Email.MaxLength);

        RuleFor(x => x.Phone)
            .MaximumLength(Employee.PhoneMaxLength)
            .Matches(@"^\+?[0-9\s\-()]+$").WithMessage("Phone number is not valid.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Salary).InclusiveBetween(0, Employee.MaxSalary);

        RuleFor(x => x.HireDate).NotEmpty();

        RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("Department is required.");
    }
}

internal sealed class EmployeeQueryValidator : AbstractValidator<EmployeeQuery>
{
    public EmployeeQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(100);

        RuleFor(x => x.SortBy).IsInEnum();

        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize).InclusiveBetween(1, EmployeeQuery.MaxPageSize);
    }
}
