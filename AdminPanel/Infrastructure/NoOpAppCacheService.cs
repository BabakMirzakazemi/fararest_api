using Services.Contracts.Caching;

namespace AdminPanel.Infrastructure;

// AdminPanel fallback cache implementation.
// It keeps service constructors satisfied without introducing external cache dependencies.
public sealed class NoOpAppCacheService : IAppCacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        => Task.FromResult<T?>(default);

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
