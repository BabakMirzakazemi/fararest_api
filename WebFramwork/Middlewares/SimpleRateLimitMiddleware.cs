using System.Collections.Concurrent;
using Common.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace WebFramework.Middlewares;

public class SimpleRateLimitMiddleware(
    RequestDelegate next,
    IOptions<PerformanceSettings> performanceOptions)
{
    // In-process counters for fixed-window throttling per client key.
    private static readonly ConcurrentDictionary<string, RateLimitWindowCounter> Counters = new();

    public async Task InvokeAsync(HttpContext context)
    {
        var rateSettings = performanceOptions.Value.RateLimiting;
        if (!rateSettings.Enabled)
        {
            await next(context);
            return;
        }

        var key = BuildClientKey(context);
        var now = DateTimeOffset.UtcNow;
        var window = TimeSpan.FromSeconds(Math.Max(1, rateSettings.WindowSeconds));
        var limit = Math.Max(1, rateSettings.PermitLimit);

        // Fixed window strategy: reset bucket when window expires, otherwise increment request count.
        var counter = Counters.AddOrUpdate(
            key,
            _ => new RateLimitWindowCounter(1, now.Add(window)),
            (_, existing) =>
            {
                if (existing.WindowEndUtc <= now)
                    return new RateLimitWindowCounter(1, now.Add(window));

                return existing with { Count = existing.Count + 1 };
            });

        if (counter.Count > limit)
        {
            var retryAfter = Math.Max(1, (int)(counter.WindowEndUtc - now).TotalSeconds);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers.RetryAfter = retryAfter.ToString();
            // Uniform JSON response makes client-side backoff behavior straightforward.
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Too many requests. Please retry later.",
                retryAfterSeconds = retryAfter
            });
            return;
        }

        await next(context);
    }

    private static string BuildClientKey(HttpContext context)
    {
        // Prefer authenticated user id for fair throttling; fallback to IP for anonymous traffic.
        var userId = context.User.FindFirst("sub")?.Value
                     ?? context.User.FindFirst("nameid")?.Value;
        if (!string.IsNullOrWhiteSpace(userId))
            return $"user:{userId}";

        var ip = context.Connection.RemoteIpAddress?.ToString();
        return string.IsNullOrWhiteSpace(ip) ? "ip:unknown" : $"ip:{ip}";
    }

    private sealed record RateLimitWindowCounter(int Count, DateTimeOffset WindowEndUtc);
}
