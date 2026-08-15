using System.Diagnostics;
using System.Net;
using NetworkDiagnosticsPOC.Interfaces;
using NetworkDiagnosticsPOC.Models;

namespace NetworkDiagnosticsPOC.Checks;

public sealed class DnsCheck : INetworkCheck
{
    public NetworkCheckType CheckType =>
        NetworkCheckType.Dns;

    public async Task<NetworkCheckResult> ExecuteAsync(
        string target,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            return new NetworkCheckResult
            {
                CheckType = CheckType,
                Target = target,
                IsSuccess = false,
                Status = "Failed",
                Details = "Domain is required."
            };
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            IPAddress[] addresses =
                await Dns.GetHostAddressesAsync(
                    target,
                    cancellationToken);

            stopwatch.Stop();

            string[] resolvedAddresses =
                addresses
                    .Select(address => address.ToString())
                    .Distinct()
                    .ToArray();

            return new NetworkCheckResult
            {
                CheckType = CheckType,
                Target = target,
                IsSuccess = resolvedAddresses.Length > 0,
                Status = resolvedAddresses.Length > 0
                    ? "Resolved"
                    : "No addresses found",
                ResponseTimeMs =
                    stopwatch.ElapsedMilliseconds,
                Details =
                    $"Resolved {resolvedAddresses.Length} address(es).",
                Addresses = resolvedAddresses
            };
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();

            return new NetworkCheckResult
            {
                CheckType = CheckType,
                Target = target,
                IsSuccess = false,
                Status = "Cancelled",
                Details = "DNS lookup was cancelled."
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            return new NetworkCheckResult
            {
                CheckType = CheckType,
                Target = target,
                IsSuccess = false,
                Status = "Error",
                Details = ex.Message
            };
        }
    }
}