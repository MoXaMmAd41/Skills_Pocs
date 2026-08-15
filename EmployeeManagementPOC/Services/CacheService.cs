using System.Text.Json;
using EmployeeManagementPOC.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace EmployeeManagementPOC.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        string? cachedValue = await _cache.GetStringAsync(key);

        if (string.IsNullOrWhiteSpace(cachedValue))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(
            cachedValue,
            JsonOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration)
    {
        string serializedValue =
            JsonSerializer.Serialize(value, JsonOptions);

        DistributedCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        await _cache.SetStringAsync(
            key,
            serializedValue,
            options);
    }

    public Task RemoveAsync(string key)
    {
        return _cache.RemoveAsync(key);
    }
}