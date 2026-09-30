using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Features.Auth;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Infrastructure.Identity;

internal sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    JwtTokenGenerator tokenGenerator) : IAuthService
{
    private const string InvalidCredentialsCode = "Auth.InvalidCredentials";
    private const string LockedOutCode = "Auth.LockedOut";

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ApplicationUser user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new AuthenticationFailedException(InvalidCredentialsCode, "Invalid email or password.");

        SignInResult result = await signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            throw new AuthenticationFailedException(LockedOutCode, "Your account is locked. Try again later.");
        }

        if (!result.Succeeded)
        {
            throw new AuthenticationFailedException(InvalidCredentialsCode, "Invalid email or password.");
        }

        IList<string> roles = await userManager.GetRolesAsync(user);
        (string token, DateTimeOffset expiresAt) = tokenGenerator.Generate(user, roles);

        return new AuthResponse(token, expiresAt, ToCurrentUser(user, roles));
    }

    public async Task<CurrentUser> GetCurrentUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        ApplicationUser user = await userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("Auth.UserNotFound", "The current user no longer exists.");

        return ToCurrentUser(user, await userManager.GetRolesAsync(user));
    }

    private static CurrentUser ToCurrentUser(ApplicationUser user, IList<string> roles) =>
        new(user.Id, user.Email ?? string.Empty, user.FullName, roles.ToList());
}
