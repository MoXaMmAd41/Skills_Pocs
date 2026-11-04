namespace DesignPatterns.Proxy.Problem;

/// <summary>
/// The naive version: security, caching and the actual remote call all live in ONE class.
/// Compare with Solution/: the remote service, the interface, and the proxy are separate.
/// </summary>
public sealed class ExchangeRateService
{
    private static readonly Dictionary<string, decimal> UsdRates = new()
    {
        ["USD"] = 1m,
        ["JOD"] = 0.709m,
        ["SAR"] = 3.75m,
        ["EUR"] = 0.92m
    };

    private readonly Dictionary<string, (decimal Rate, DateTime ExpiresAt)> _cache = [];

    public decimal GetRate(string from, string to, string userRole)
    {
        // ❌ 1. Security is mixed into the business class. Every new method has to remember to copy this check.
        // ❌ 2. The role has to be passed into every call, so the method signature is polluted with security details.
        if (userRole is not ("Finance" or "Admin"))
        {
            throw new UnauthorizedAccessException($"Role '{userRole}' cannot read exchange rates.");
        }

        string key = $"{from}/{to}";

        // ❌ 3. Caching is mixed in too, and it uses DateTime.Now directly, so expiry can't be tested without waiting.
        if (_cache.TryGetValue(key, out var cached) && cached.ExpiresAt > DateTime.Now)
        {
            return cached.Rate;
        }

        // ❌ 4. The real work is buried at the bottom. A background job that should always get fresh rates
        //       and has no user role can't call the remote API without also going through the cache and the role check.
        Thread.Sleep(500); // the slow, paid remote call

        if (!UsdRates.TryGetValue(from, out decimal fromUsd) || !UsdRates.TryGetValue(to, out decimal toUsd))
        {
            throw new ArgumentException($"Unsupported currency pair {from}/{to}.");
        }

        decimal rate = decimal.Round(toUsd / fromUsd, 4);
        _cache[key] = (rate, DateTime.Now.AddMinutes(5));

        return rate;
    }
}
