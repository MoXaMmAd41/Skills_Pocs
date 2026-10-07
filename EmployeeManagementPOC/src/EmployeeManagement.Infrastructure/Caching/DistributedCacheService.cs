using System.Text.Json;
using EmployeeManagement.Application.Common.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Infrastructure.Caching;

/// <summary>
/// Cache-aside over <see cref="IDistributedCache"/>. The cache is an optimisation, not a dependency:
/// if the backing store is unreachable the request falls through to the source of truth.
/// </summary>
internal sealed class DistributedCacheService(
    IDistributedCache cache,
    ILogger<DistributedCacheService> logger) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        T? cached = await TryGetAsync<T>(key, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        T value = await factory(cancellationToken);

        await TrySetAsync(key, value, expiration, cancellationToken);

        return value;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Failed to evict cache entry {CacheKey}.", key);
        }
    }

    private async Task<T?> TryGetAsync<T>(string key, CancellationToken cancellationToken)
    {
        try
        {
            byte[]? payload = await cache.GetAsync(key, cancellationToken);

            return payload is null ? default : JsonSerializer.Deserialize<T>(payload, JsonOptions);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Failed to read cache entry {CacheKey}; falling back to source.", key);
            return default;
        }
    }

    private async Task TrySetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken)
    {
        try
        {
            byte[] payload = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);

            await cache.SetAsync(
                key,
                payload,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration },
                cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Failed to write cache entry {CacheKey}.", key);
        }
    }
}
