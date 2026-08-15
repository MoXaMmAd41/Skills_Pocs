using Microsoft.Extensions.DependencyInjection;
using NetworkDiagnosticsPOC.Checks;
using NetworkDiagnosticsPOC.Interfaces;
using NetworkDiagnosticsPOC.Models;
using NetworkDiagnosticsPOC.Services;

ServiceCollection services = new();

services.AddHttpClient();

services.AddSingleton<INetworkCheck, PingCheck>();

services.AddSingleton<INetworkCheck, DnsCheck>();

services.AddSingleton<INetworkCheck>(serviceProvider =>
{
    IHttpClientFactory httpClientFactory =
        serviceProvider
            .GetRequiredService<IHttpClientFactory>();

    HttpClient httpClient =
        httpClientFactory.CreateClient();

    return new HttpCheck(httpClient);
});

services.AddSingleton<IOutputFormatter, ConsoleOutputFormatter>();

services.AddSingleton<NetworkDiagnosticsService>();

services.AddSingleton<ConsoleMenuService>();

using ServiceProvider serviceProvider =
    services.BuildServiceProvider();

NetworkDiagnosticsService diagnosticsService =
    serviceProvider
        .GetRequiredService<NetworkDiagnosticsService>();

ConsoleMenuService menuService =
    serviceProvider
        .GetRequiredService<ConsoleMenuService>();

IOutputFormatter outputFormatter =
    serviceProvider
        .GetRequiredService<IOutputFormatter>();

while (true)
{
    menuService.ShowMenu();

    string option =
        menuService.ReadOption();

    if (option == "5")
    {
        Console.WriteLine(
            "Exiting Network Diagnostics Tool...");

        break;
    }

    if (option == "4")
    {
        await RunAllChecksAsync();

        continue;
    }

    NetworkCheckType? checkType =
        menuService.ParseCheckType(option);

    if (!checkType.HasValue)
    {
        Console.WriteLine(
            "Invalid option.");

        continue;
    }

    string target =
        menuService.ReadTarget(
            checkType.Value);

    NetworkCheckResult result =
        await diagnosticsService.RunAsync(
            checkType.Value,
            target);

    outputFormatter.PrintResult(result);
}

async Task RunAllChecksAsync()
{
    string target =
        menuService.ReadAllChecksTarget();

    if (string.IsNullOrWhiteSpace(target))
    {
        Console.WriteLine(
            "Target is required.");

        return;
    }

    string normalizedHost =
        NormalizeHost(target);

    string httpTarget =
        NormalizeHttpUrl(target);

    List<NetworkCheckResult> results =
    [
        await diagnosticsService.RunAsync(
            NetworkCheckType.Ping,
            normalizedHost),

        await diagnosticsService.RunAsync(
            NetworkCheckType.Dns,
            normalizedHost),

        await diagnosticsService.RunAsync(
            NetworkCheckType.Http,
            httpTarget)
    ];

    outputFormatter.PrintResults(results);
}

static string NormalizeHost(string target)
{
    if (Uri.TryCreate(
            target,
            UriKind.Absolute,
            out Uri? uri))
    {
        return uri.Host;
    }

    return target;
}

static string NormalizeHttpUrl(string target)
{
    if (target.StartsWith(
            "http://",
            StringComparison.OrdinalIgnoreCase) ||
        target.StartsWith(
            "https://",
            StringComparison.OrdinalIgnoreCase))
    {
        return target;
    }

    return $"https://{target}";
}