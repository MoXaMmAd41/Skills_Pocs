using DesignPatterns.Proxy.Solution.Interfaces;

namespace DesignPatterns.Proxy.Solution.Services;

/// <summary>The real subject: pretends to be a slow, paid external API.</summary>
public sealed class RemoteExchangeRateService(TimeSpan latency) : IExchangeRateService
{
    private static readonly Dictionary<string, decimal> UsdRates = new()
    {
        ["USD"] = 1m,
        ["JOD"] = 0.709m,
        ["SAR"] = 3.75m,
        ["EUR"] = 0.92m
    };

    public int Calls { get; private set; }

    public decimal GetRate(string from, string to)
    {
        Calls++;
        Thread.Sleep(latency);

        if (!UsdRates.TryGetValue(from, out decimal fromUsd) || !UsdRates.TryGetValue(to, out decimal toUsd))
        {
            throw new ArgumentException($"Unsupported currency pair {from}/{to}.");
        }

        return decimal.Round(toUsd / fromUsd, 4);
    }
}
