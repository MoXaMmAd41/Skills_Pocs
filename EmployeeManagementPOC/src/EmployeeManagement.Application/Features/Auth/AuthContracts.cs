using FluentValidation;

namespace EmployeeManagement.Application.Features.Auth;

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthResponse(string AccessToken, DateTimeOffset ExpiresAt, CurrentUser User);

public sealed record CurrentUser(string Id, string Email, string FullName, IReadOnlyList<string> Roles);

/// <summary>
/// Implemented by the identity provider in Infrastructure.
/// </summary>
public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<CurrentUser> GetCurrentUserAsync(string userId, CancellationToken cancellationToken = default);
}

internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.Password).NotEmpty();
    }
}
