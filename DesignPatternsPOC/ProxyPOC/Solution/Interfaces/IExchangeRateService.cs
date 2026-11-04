namespace DesignPatterns.Proxy.Solution.Interfaces;

/// <summary>The subject: both the real service and the proxy implement it, so callers can't tell them apart.</summary>
public interface IExchangeRateService
{
    decimal GetRate(string from, string to);
}
