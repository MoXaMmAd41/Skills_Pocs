using DesignPatterns.Proxy.Solution.Interfaces;

namespace DesignPatterns.Proxy.Solution.Proxies;

/// <summary>
/// Proxy = stands in front of the real object and controls access to it.
/// This one does two jobs without the real service knowing:
/// protection (only allowed roles) and caching (don't pay for the same answer twice).
/// </summary>
public sealed class ExchangeRateProxy(
    IExchangeRateService inner,
    string userRole,
    TimeProvider clock,
    TimeSpan cacheFor) : IExchangeRateService
{
    private static readonly HashSet<string> AllowedRoles = ["Finance", "Admin"];

    private readonly Dictionary<string, (decimal Rate, DateTimeOffset ExpiresAt)> _cache = [];

    public int CacheHits { get; private set; }

    public decimal GetRate(string from, string to)
    {
        if (!AllowedRoles.Contains(userRole))
        {
            throw new UnauthorizedAccessException($"Role '{userRole}' cannot read exchange rates.");
        }

        string key = $"{from.ToUpperInvariant()}/{to.ToUpperInvariant()}";

        if (_cache.TryGetValue(key, out var cached) && cached.ExpiresAt > clock.GetUtcNow())
        {
            CacheHits++;
            return cached.Rate;
        }

        decimal rate = inner.GetRate(from.ToUpperInvariant(), to.ToUpperInvariant());
        _cache[key] = (rate, clock.GetUtcNow() + cacheFor);

        return rate;
    }
}
