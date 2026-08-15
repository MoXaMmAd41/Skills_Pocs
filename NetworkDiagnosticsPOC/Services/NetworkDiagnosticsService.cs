using NetworkDiagnosticsPOC.Interfaces;
using NetworkDiagnosticsPOC.Models;

namespace NetworkDiagnosticsPOC.Services;

public sealed class NetworkDiagnosticsService
{
    private readonly Dictionary<
        NetworkCheckType,
        INetworkCheck> _checks;

    public NetworkDiagnosticsService(
        IEnumerable<INetworkCheck> checks)
    {
        _checks = checks.ToDictionary(
            check => check.CheckType,
            check => check);
    }

    public async Task<NetworkCheckResult> RunAsync(
        NetworkCheckType checkType,
        string target,
        CancellationToken cancellationToken = default)
    {
        if (!_checks.TryGetValue(
                checkType,
                out INetworkCheck? check))
        {
            throw new InvalidOperationException(
                $"No network check registered for {checkType}.");
        }

        return await check.ExecuteAsync(
            target,
            cancellationToken);
    }
}