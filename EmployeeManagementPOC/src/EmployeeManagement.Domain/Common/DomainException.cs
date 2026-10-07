namespace EmployeeManagement.Domain.Common;

/// <summary>
/// Raised when an operation would violate a business invariant.
/// </summary>
public sealed class DomainException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}
