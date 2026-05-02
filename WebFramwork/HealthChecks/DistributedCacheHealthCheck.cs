using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebFramework.HealthChecks;

public class DistributedCacheHealthCheck(IDistributedCache distributedCache) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // A lightweight write/read ping verifies that cache is usable for real runtime operations.
        const string key = "health:cache:ping";
        const string value = "ok";

        try
        {
            await distributedCache.SetStringAsync(
                key,
                value,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) },
                cancellationToken);

            var result = await distributedCache.GetStringAsync(key, cancellationToken);
            return result == value
                ? HealthCheckResult.Healthy("Distributed cache is reachable.")
                : HealthCheckResult.Unhealthy("Distributed cache returned invalid data.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Distributed cache is not reachable.", ex);
        }
    }
}
