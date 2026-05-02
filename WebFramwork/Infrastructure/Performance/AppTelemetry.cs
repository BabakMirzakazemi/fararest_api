using Common.Configurations;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace WebFramework.Infrastructure.Performance;

// Shared telemetry primitives for request tracing/metrics across middleware/services.
public sealed class AppTelemetry : IDisposable
{
    public ActivitySource ActivitySource { get; }
    public Meter Meter { get; }
    public Histogram<double> HttpServerRequestDurationMs { get; }
    public Counter<long> HttpServerRequestsTotal { get; }
    public Counter<long> HttpServerErrorsTotal { get; }
    public Histogram<long> HttpServerRequestSizeBytes { get; }
    public Histogram<long> HttpServerResponseSizeBytes { get; }

    public AppTelemetry(IOptions<PerformanceSettings> performanceOptions)
    {
        var settings = performanceOptions.Value.Telemetry;
        var activitySourceName = string.IsNullOrWhiteSpace(settings.ActivitySourceName)
            ? "babak_base.api"
            : settings.ActivitySourceName;
        var meterName = string.IsNullOrWhiteSpace(settings.MeterName)
            ? "babak_base.api"
            : settings.MeterName;

        ActivitySource = new ActivitySource(activitySourceName);
        Meter = new Meter(meterName, "1.0.0");

        HttpServerRequestDurationMs = Meter.CreateHistogram<double>(
            "http.server.request.duration.ms",
            unit: "ms",
            description: "HTTP request duration in milliseconds.");

        HttpServerRequestsTotal = Meter.CreateCounter<long>(
            "http.server.requests.total",
            unit: "{request}",
            description: "Total HTTP requests processed by API.");

        HttpServerErrorsTotal = Meter.CreateCounter<long>(
            "http.server.errors.total",
            unit: "{request}",
            description: "Total HTTP requests that ended with 5xx or unhandled exception.");

        HttpServerRequestSizeBytes = Meter.CreateHistogram<long>(
            "http.server.request.size.bytes",
            unit: "By",
            description: "Incoming HTTP request body size.");

        HttpServerResponseSizeBytes = Meter.CreateHistogram<long>(
            "http.server.response.size.bytes",
            unit: "By",
            description: "Outgoing HTTP response body size.");
    }

    public void Dispose()
    {
        ActivitySource.Dispose();
        Meter.Dispose();
    }
}
