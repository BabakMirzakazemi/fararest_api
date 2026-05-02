using Common.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using WebFramework.Infrastructure.Performance;

namespace WebFramework.Middlewares;

// Captures request-level traces and core performance metrics.
// Designed to stay fail-open (never block user response due telemetry).
public class RequestTelemetryMiddleware(
    RequestDelegate next,
    IOptions<PerformanceSettings> performanceOptions,
    AppTelemetry telemetry)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var settings = performanceOptions.Value.Telemetry;
        if (!settings.Enabled)
        {
            await next(context);
            return;
        }

        if (IsExcludedPath(context.Request.Path, settings.ExcludedPaths))
        {
            await next(context);
            return;
        }

        var requestStart = Stopwatch.GetTimestamp();
        Exception? unhandledException = null;
        // Do not create a new server activity here; ASP.NET OpenTelemetry instrumentation already does it.
        // Reusing current activity prevents duplicate spans and keeps overhead minimal.
        var activity = Activity.Current;
        activity?.SetTag("service.name", settings.ServiceName);
        activity?.SetTag("http.request.method", context.Request.Method);
        activity?.SetTag("url.path", context.Request.Path.Value);
        activity?.SetTag("url.query", context.Request.QueryString.Value);
        activity?.SetTag("server.address", context.Request.Host.Host);
        activity?.SetTag("server.port", context.Request.Host.Port);

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            unhandledException = ex;
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().FullName);
            throw;
        }
        finally
        {
            var elapsedMs = Stopwatch.GetElapsedTime(requestStart).TotalMilliseconds;
            var endpointName = context.GetEndpoint()?.DisplayName ?? "unmatched";
            var statusCode = context.Response.StatusCode;

            activity?.SetTag("http.route", endpointName);
            activity?.SetTag("http.response.status_code", statusCode);
            if (statusCode >= StatusCodes.Status500InternalServerError)
                activity?.SetStatus(ActivityStatusCode.Error);

            if (!context.Response.HasStarted && settings.AddTraceIdHeader)
            {
                var headerName = string.IsNullOrWhiteSpace(settings.TraceIdHeaderName)
                    ? "X-Trace-Id"
                    : settings.TraceIdHeaderName;
                var traceId = activity?.TraceId.ToString();
                if (!string.IsNullOrWhiteSpace(traceId))
                    context.Response.Headers[headerName] = traceId;
            }

            var tags = new TagList
            {
                { "service.name", settings.ServiceName },
                { "http.method", context.Request.Method },
                { "http.route", endpointName },
                { "http.status_code", statusCode },
                { "http.status_class", $"{statusCode / 100}xx" }
            };
            // TODO(Telemetry): add business/KPI tags (tenant, product-area, feature-flag) per endpoint after domain flows stabilize.

            telemetry.HttpServerRequestsTotal.Add(1, tags);
            telemetry.HttpServerRequestDurationMs.Record(elapsedMs, tags);

            if (context.Request.ContentLength.HasValue)
                telemetry.HttpServerRequestSizeBytes.Record(context.Request.ContentLength.Value, tags);

            if (context.Response.ContentLength.HasValue)
                telemetry.HttpServerResponseSizeBytes.Record(context.Response.ContentLength.Value, tags);

            if (unhandledException != null || statusCode >= StatusCodes.Status500InternalServerError)
                telemetry.HttpServerErrorsTotal.Add(1, tags);
        }
    }

    private static bool IsExcludedPath(PathString requestPath, IReadOnlyCollection<string>? excludedPaths)
    {
        if (excludedPaths == null || excludedPaths.Count == 0)
            return false;

        foreach (var raw in excludedPaths)
        {
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            var candidate = raw.Trim();
            if (requestPath.StartsWithSegments(candidate, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
