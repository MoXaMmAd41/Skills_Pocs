using NetworkDiagnosticsPOC.Models;

namespace NetworkDiagnosticsPOC.Interfaces;

public interface INetworkCheck
{
    NetworkCheckType CheckType { get; }

    Task<NetworkCheckResult> ExecuteAsync(
        string target,
        CancellationToken cancellationToken = default);
}