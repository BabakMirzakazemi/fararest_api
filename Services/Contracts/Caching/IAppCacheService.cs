namespace Services.Contracts.Caching;

// Service-layer cache abstraction. Implementation details (Redis/Memory) stay in infrastructure.
public interface IAppCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
