using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Services.Contracts.Caching;
using System.Threading;

namespace WebFramework.Infrastructure.Caching;

public class DistributedAppCacheService(
    IDistributedCache distributedCache,
    ILogger<DistributedAppCacheService> logger) : IAppCacheService
{
    // Shared serializer options to keep payload format consistent across set/get.
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private const int BackendFailureCooldownSeconds = 30;
    private long _cacheBackendUnavailableUntilUnixSeconds;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (IsInFailureCooldown())
            return default;

        string? payload;
        try
        {
            payload = await distributedCache.GetStringAsync(key, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            MarkBackendUnavailable(ex, "Get", key);
            return default;
        }

        if (string.IsNullOrWhiteSpace(payload))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(payload, SerializerOptions);
        }
        catch (JsonException ex)
        {
            // If cache payload is corrupted, schema has changed, or constructor binding fails,
            // remove the stale entry and fallback to source of truth.
            logger.LogWarning(ex, "Failed to deserialize cache value. Key: {CacheKey}", key);
            await RemoveAsync(key, cancellationToken);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        if (IsInFailureCooldown())
            return;

        var payload = JsonSerializer.Serialize(value, SerializerOptions);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };

        try
        {
            await distributedCache.SetStringAsync(key, payload, options, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            MarkBackendUnavailable(ex, "Set", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (IsInFailureCooldown())
            return;

        try
        {
            await distributedCache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            MarkBackendUnavailable(ex, "Remove", key);
        }
    }

    private bool IsInFailureCooldown()
    {
        var unavailableUntil = Interlocked.Read(ref _cacheBackendUnavailableUntilUnixSeconds);
        if (unavailableUntil <= 0)
            return false;

        return DateTimeOffset.UtcNow.ToUnixTimeSeconds() < unavailableUntil;
    }

    private void MarkBackendUnavailable(Exception ex, string operationName, string cacheKey)
    {
        var unavailableUntil = DateTimeOffset.UtcNow.AddSeconds(BackendFailureCooldownSeconds).ToUnixTimeSeconds();
        Interlocked.Exchange(ref _cacheBackendUnavailableUntilUnixSeconds, unavailableUntil);

        logger.LogWarning(
            ex,
            "Cache backend unavailable in {Operation}. Key: {CacheKey}. Caching is bypassed for {CooldownSeconds} seconds.",
            operationName,
            cacheKey,
            BackendFailureCooldownSeconds);
    }
}
