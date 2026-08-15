namespace NetworkDiagnosticsPOC.Models;

public sealed class NetworkCheckResult
{
    public NetworkCheckType CheckType { get; init; }

    public string Target { get; init; } = string.Empty;

    public bool IsSuccess { get; init; }

    public string Status { get; init; } = string.Empty;

    public long? ResponseTimeMs { get; init; }

    public string? Details { get; init; }

    public IReadOnlyCollection<string> Addresses { get; init; }
        = Array.Empty<string>();
}