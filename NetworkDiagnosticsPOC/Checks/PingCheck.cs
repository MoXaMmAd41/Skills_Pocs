using System.Diagnostics;
using System.Net.NetworkInformation;
using NetworkDiagnosticsPOC.Interfaces;
using NetworkDiagnosticsPOC.Models;

namespace NetworkDiagnosticsPOC.Checks;

public sealed class PingCheck : INetworkCheck
{
    public NetworkCheckType CheckType =>
        NetworkCheckType.Ping;

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
                Details = "Target is required."
            };
        }

        using Ping ping = new();

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            PingReply reply =
                await ping.SendPingAsync(
                    target,
                    3000);

            stopwatch.Stop();

            bool success =
                reply.Status == IPStatus.Success;

            return new NetworkCheckResult
            {
                CheckType = CheckType,
                Target = target,
                IsSuccess = success,
                Status = reply.Status.ToString(),
                ResponseTimeMs = success
                    ? stopwatch.ElapsedMilliseconds
                    : null,
                Details = success
                    ? $"Reply from {target}"
                    : $"Ping failed with status {reply.Status}"
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
                Details = "Ping operation was cancelled."
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