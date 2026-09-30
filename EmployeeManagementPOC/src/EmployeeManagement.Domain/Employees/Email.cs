using System.Net.Mail;
using EmployeeManagement.Domain.Common;

namespace EmployeeManagement.Domain.Employees;

public sealed record Email
{
    public const int MaxLength = 150;

    private Email(string value) => Value = value;

    public string Value { get; }

    public static Email Create(string? value)
    {
        string normalized = value?.Trim().ToLowerInvariant() ?? string.Empty;

        if (normalized.Length is 0 or > MaxLength || !IsWellFormed(normalized))
        {
            throw new DomainException(EmployeeErrorCodes.InvalidEmail, "Email address is not valid.");
        }

        return new Email(normalized);
    }

    public override string ToString() => Value;

    private static bool IsWellFormed(string value) =>
        MailAddress.TryCreate(value, out MailAddress? address) && address.Address == value;
}
