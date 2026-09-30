using FluentValidation;

namespace EmployeeManagement.Application.Features.Employees;

public sealed record EmployeeSuggestion(int Id, string FullName, string Email, string DepartmentName);

public sealed record EmployeeSuggestionQuery
{
    public const int MaxLimit = 20;

    public string Prefix { get; init; } = string.Empty;

    public int Limit { get; init; } = 8;
}

/// <summary>
/// In-memory prefix index over employee names and emails, used for search-as-you-type.
/// Must be invalidated whenever employees change.
/// </summary>
public interface IEmployeeSearchIndex
{
    Task<IReadOnlyList<EmployeeSuggestion>> SuggestAsync(
        string prefix,
        int limit,
        CancellationToken cancellationToken = default);

    void Invalidate();
}

internal sealed class EmployeeSuggestionQueryValidator : AbstractValidator<EmployeeSuggestionQuery>
{
    public EmployeeSuggestionQueryValidator()
    {
        RuleFor(x => x.Prefix).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Limit).InclusiveBetween(1, EmployeeSuggestionQuery.MaxLimit);
    }
}
