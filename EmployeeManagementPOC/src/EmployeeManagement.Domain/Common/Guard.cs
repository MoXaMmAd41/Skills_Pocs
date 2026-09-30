namespace EmployeeManagement.Domain.Common;

internal static class Guard
{
    public static string RequiredText(string? value, int maxLength, string code, string fieldName)
    {
        string trimmed = value?.Trim() ?? string.Empty;

        if (trimmed.Length == 0)
        {
            throw new DomainException(code, $"{fieldName} is required.");
        }

        if (trimmed.Length > maxLength)
        {
            throw new DomainException(code, $"{fieldName} must not exceed {maxLength} characters.");
        }

        return trimmed;
    }

    public static string? OptionalText(string? value, int maxLength, string code, string fieldName)
    {
        string? trimmed = string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        if (trimmed?.Length > maxLength)
        {
            throw new DomainException(code, $"{fieldName} must not exceed {maxLength} characters.");
        }

        return trimmed;
    }
}
