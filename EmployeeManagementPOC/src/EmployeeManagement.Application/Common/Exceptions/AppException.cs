namespace EmployeeManagement.Application.Common.Exceptions;

/// <summary>
/// Base type for expected, client-facing application failures.
/// The API layer maps each concrete type to an HTTP status code.
/// </summary>
public abstract class AppException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}

public sealed class NotFoundException(string code, string message) : AppException(code, message);

public sealed class ConflictException(string code, string message) : AppException(code, message);

public sealed class AuthenticationFailedException(string code, string message) : AppException(code, message);
