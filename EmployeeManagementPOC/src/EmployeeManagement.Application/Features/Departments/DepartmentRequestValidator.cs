using EmployeeManagement.Domain.Departments;
using FluentValidation;

namespace EmployeeManagement.Application.Features.Departments;

internal sealed class DepartmentRequestValidator : AbstractValidator<DepartmentRequest>
{
    public DepartmentRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Department.NameMaxLength);

        RuleFor(x => x.Description).MaximumLength(Department.DescriptionMaxLength);
    }
}
