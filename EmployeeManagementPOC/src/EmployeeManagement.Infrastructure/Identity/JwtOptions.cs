using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Infrastructure.Identity;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// HMAC-SHA256 key; must be at least 32 characters. Supply it via secrets/environment, never source control.
    /// </summary>
    [Required, MinLength(32)]
    public string SigningKey { get; init; } = string.Empty;

    [Range(typeof(TimeSpan), "00:05:00", "1.00:00:00")]
    public TimeSpan AccessTokenLifetime { get; init; } = TimeSpan.FromHours(8);
}
