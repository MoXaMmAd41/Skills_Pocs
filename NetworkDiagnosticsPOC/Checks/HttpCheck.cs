using System.Diagnostics;
using NetworkDiagnosticsPOC.Interfaces;
using NetworkDiagnosticsPOC.Models;

namespace NetworkDiagnosticsPOC.Checks;

public sealed class HttpCheck : INetworkCheck
{
    private readonly HttpClient _httpClient;

    public HttpCheck(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public NetworkCheckType CheckType =>
        NetworkCheckType.Http;

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
                Details = "URL is required."
            };
        }

        if (!Uri.TryCreate(
                target,
                UriKind.Absolute,
                out Uri? uri) ||
            (uri.Scheme != Uri.UriSchemeHttp &&
             uri.Scheme != Uri.UriSchemeHttps))
        {
            return new NetworkCheckResult
            {
                CheckType = CheckType,
                Target = target,
                IsSuccess = false,
                Status = "Invalid URL",
                Details =
                    "The URL must start with http:// or https://."
            };
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            using HttpResponseMessage response =
                await _httpClient.GetAsync(
                    uri,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

            stopwatch.Stop();

            return new NetworkCheckResult
            {
                CheckType = CheckType,
                Target = target,
                IsSuccess = response.IsSuccessStatusCode,
                Status =
                    $"{(int)response.StatusCode} {response.StatusCode}",
                ResponseTimeMs =
                    stopwatch.ElapsedMilliseconds,
                Details =
                    response.IsSuccessStatusCode
                        ? "HTTP request completed successfully."
                        : "HTTP request returned a non-success status."
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
                Details = "HTTP request was cancelled."
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