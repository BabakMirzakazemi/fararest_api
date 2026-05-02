namespace Common.Configurations;

// Centralized performance knobs. This keeps operational tuning خارج از کد بیزینس.
public class PerformanceSettings
{
    public CachingSettings Caching { get; set; } = new();
    public RateLimitingSettings RateLimiting { get; set; } = new();
    public ObservabilitySettings Observability { get; set; } = new();
    public QueryTuningSettings QueryTuning { get; set; } = new();
    public TelemetrySettings Telemetry { get; set; } = new();
}

public class CachingSettings
{
    // Master switch for app-level caching behavior.
    public bool Enabled { get; set; } = true;
    // If true and RedisConnection is set, distributed Redis cache will be used.
    public bool UseRedis { get; set; }
    public string RedisConnection { get; set; } = string.Empty;
    public string RedisInstanceName { get; set; } = "babak_base:";
    // Default TTL for general cached endpoints.
    public int DefaultTtlSeconds { get; set; } = 60;
    // Endpoint-specific TTL for users list query.
    public int UsersListTtlSeconds { get; set; } = 45;
}

public class RateLimitingSettings
{
    // Master switch for request throttling.
    public bool Enabled { get; set; } = true;
    // Max accepted requests per window for each key (user/ip).
    public int PermitLimit { get; set; } = 120;
    // Window size used by limiter.
    public int WindowSeconds { get; set; } = 60;
    // Reserved for future queue-based limiter strategy.
    public int QueueLimit { get; set; } = 20;
}

public class ObservabilitySettings
{
    // If true, a correlation id header is returned in response.
    public bool AddCorrelationIdHeader { get; set; } = true;
    public string CorrelationIdHeaderName { get; set; } = "X-Correlation-ID";
}

public class QueryTuningSettings
{
    // Master switch for query observability hooks (slow query logging, query tags).
    public bool Enabled { get; set; } = true;
    // Log queries slower than this threshold.
    public int SlowQueryThresholdMs { get; set; } = 250;
    // Include SQL text in warning logs for diagnosis (disable in highly sensitive environments).
    public bool LogCommandText { get; set; } = true;
}

public class TelemetrySettings
{
    // Master switch for request tracing and metrics collection.
    public bool Enabled { get; set; } = true;
    // Logical service name used in telemetry tags and activity source.
    public string ServiceName { get; set; } = "babak_base.api";
    // ActivitySource name; keep stable for downstream collectors/dashboards.
    public string ActivitySourceName { get; set; } = "babak_base.api";
    // Meter name used by runtime metrics listeners.
    public string MeterName { get; set; } = "babak_base.api";
    // Echo W3C trace-id to response header for faster support/debug workflows.
    public bool AddTraceIdHeader { get; set; } = true;
    public string TraceIdHeaderName { get; set; } = "X-Trace-Id";
    // Exposes Prometheus scrape endpoint on API process (for Grafana live charts via Prometheus).
    public bool EnablePrometheusScrapingEndpoint { get; set; } = true;
    public string PrometheusScrapingEndpointPath { get; set; } = "/metrics";
    // Optional OTLP export endpoint for traces/metrics/log correlations (for Jaeger/Collector).
    public string OtlpEndpoint { get; set; } = string.Empty;
    // Supported: "grpc" | "http/protobuf"
    public string OtlpProtocol { get; set; } = "http/protobuf";
    // Head-based trace sampling ratio to control overhead (0..1).
    public double TraceSamplingRatio { get; set; } = 0.2;
    // High-frequency technical paths excluded from custom telemetry counters/histograms.
    public string[] ExcludedPaths { get; set; } = ["/health", "/metrics", "/swagger"];
    // Include SQL text in trace spans. Keep false by default to reduce payload/overhead.
    public bool IncludeDbStatementInTraces { get; set; }
}
