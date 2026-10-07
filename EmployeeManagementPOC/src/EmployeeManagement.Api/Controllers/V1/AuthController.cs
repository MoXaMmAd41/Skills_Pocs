using Asp.Versioning;
using EmployeeManagement.Api.Versioning;
using EmployeeManagement.Application.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace EmployeeManagement.Api.Controllers.V1;

[ApiVersion(ApiVersions.V1)]
public sealed class AuthController(IAuthService authService) : ApiControllerBase
{
    /// <summary>Exchanges credentials for a bearer token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<AuthResponse> Login(LoginRequest request, CancellationToken cancellationToken) =>
        authService.LoginAsync(request, cancellationToken);

    /// <summary>Returns the profile and roles of the authenticated user.</summary>
    [HttpGet("me")]
    [ProducesResponseType<CurrentUser>(StatusCodes.Status200OK)]
    public Task<CurrentUser> Me(CancellationToken cancellationToken) =>
        authService.GetCurrentUserAsync(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value, cancellationToken);
}
