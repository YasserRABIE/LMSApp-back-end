using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace LMS.Infrastructure.Caching;

/// <summary>
/// Redis cache service implementation
/// </summary>
public sealed class RedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(value))
            return default;

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _cache.GetStringAsync(key, cancellationToken);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();

        if (expiry.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiry.Value;
        }

        var serializedValue = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serializedValue, options, cancellationToken);
    }

    public async Task SetStringAsync(
        string key,
        string value,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();

        if (expiry.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiry.Value;
        }

        await _cache.SetStringAsync(key, value, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(value);
    }

    public async Task<int> IncrementAsync(
        string key,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default)
    {
        var currentValue = await GetAsync<int>(key, cancellationToken);
        var newValue = currentValue + 1;
        await SetAsync(key, newValue, expiry, cancellationToken);
        return newValue;
    }
}
