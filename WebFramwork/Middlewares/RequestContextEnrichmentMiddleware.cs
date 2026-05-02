using Common.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace WebFramework.Middlewares;

public class RequestContextEnrichmentMiddleware(
    RequestDelegate next,
    IOptions<PerformanceSettings> performanceOptions)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var observability = performanceOptions.Value.Observability;
        var headerName = string.IsNullOrWhiteSpace(observability.CorrelationIdHeaderName)
            ? "X-Correlation-ID"
            : observability.CorrelationIdHeaderName;

        // Use incoming correlation id when client provides one; otherwise fallback to ASP.NET trace id.
        var correlationId = context.Request.Headers.TryGetValue(headerName, out var incomingCorrelationId)
            && !string.IsNullOrWhiteSpace(incomingCorrelationId)
            ? incomingCorrelationId.ToString()
            : context.TraceIdentifier;

        // Echo correlation id so clients can correlate their request with server logs.
        if (observability.AddCorrelationIdHeader)
            context.Response.Headers[headerName] = correlationId;

        // Push correlation id into Serilog scope for all logs in current request.
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
