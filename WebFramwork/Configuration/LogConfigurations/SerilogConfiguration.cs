using Common.Configurations;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;
using System.Diagnostics;

namespace WebFramework.Configuration.LogConfigurations;

public static class SerilogConfiguration
{
    public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder builder, SeqSettings seqSettings)
    {
        builder.Host.UseSerilog();

        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Override("Default", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .MinimumLevel.Information()
            .WriteTo.Console();

        // Enable Seq sink only when URL is configured; keeps local environments flexible.
        if (seqSettings is not null && !string.IsNullOrWhiteSpace(seqSettings.SeqUrl))
            loggerConfiguration = loggerConfiguration.WriteTo.Seq(seqSettings.SeqUrl, apiKey: seqSettings.SeqApiKey);

        Log.Logger = loggerConfiguration.CreateLogger();

        return builder;
    }

    public static IApplicationBuilder UseCustomSerilogRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}";
            options.EnrichDiagnosticContext = (diagnostics, context) =>
            {
                var activity = Activity.Current;
                diagnostics.Set("TraceId", activity?.TraceId.ToString());
                diagnostics.Set("SpanId", activity?.SpanId.ToString());
                diagnostics.Set("RequestHost", context.Request.Host.Value);
                diagnostics.Set("UserAgent", context.Request.Headers.UserAgent.ToString());
            };
        });

        return app;
    }
}
