using Common.Utilities.Helpers;
using Data;
using Data.Database.SqlObjects;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using Services.DataInitializer;
using Common.Configurations;
using WebFramework.Middlewares;

namespace WebFramework.Configuration;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseHsts(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        Assert.NotNull(app, nameof(app));
        Assert.NotNull(env, nameof(env));

        if (!env.IsDevelopment())
            app.UseHsts();

        return app;
    }

    public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
    {
        Assert.NotNull(app, nameof(app));

        //Use C# 8 using variables
        using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); //Service locator

        //Dos not use Migrations, just Create Database with latest changes
        //dbContext.Database.EnsureCreated();
        //Applies any pending migrations for the context to the database like (Update-Database)
        dbContext.Database.Migrate();
        // Re-apply SQL objects (functions/triggers/views) from stable scripts.
        DatabaseSqlObjectsInstaller.ApplyAsync(dbContext).GetAwaiter().GetResult();

        var dataInitializers = scope.ServiceProvider.GetServices<IDataInitializer>().OrderBy(i => i.Order).ToList();
        Console.WriteLine($"[Seed] IDataInitializer count: {dataInitializers.Count}");
        foreach (var dataInitializer in dataInitializers)
        {
            Console.WriteLine($"[Seed] Running: {dataInitializer.GetType().FullName}");
            dataInitializer.InitializeData();
        }

        return app;
    }

    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }

    public static IApplicationBuilder UseRequestContextEnrichment(this IApplicationBuilder app)
        => app.UseMiddleware<RequestContextEnrichmentMiddleware>();

    public static IApplicationBuilder UsePerformancePipeline(this IApplicationBuilder app)
    {
        var telemetry = app.ApplicationServices.GetRequiredService<IOptions<PerformanceSettings>>().Value.Telemetry;
        var metricsPath = string.IsNullOrWhiteSpace(telemetry.PrometheusScrapingEndpointPath)
            ? "/metrics"
            : telemetry.PrometheusScrapingEndpointPath;

        // End-to-end request telemetry (trace + metrics) for bottleneck analysis.
        app.UseMiddleware<RequestTelemetryMiddleware>();

        // Prometheus scrape endpoint for Grafana live charts.
        if (telemetry.Enabled && telemetry.EnablePrometheusScrapingEndpoint)
            app.UseOpenTelemetryPrometheusScrapingEndpoint(path: metricsPath);

        // Rate limit first to protect downstream resources under burst traffic.
        app.UseMiddleware<SimpleRateLimitMiddleware>();

        // Liveness: process is alive (fast and dependency-light).
        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live")
        });

        // Readiness: critical dependencies (database/cache) are reachable.
        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        return app;
    }

    public static IApplicationBuilder UseCustomStaticFiles(this IApplicationBuilder app)
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                var url = ctx.Context.Request.Path.ToString();
                var filePath = ctx.File.PhysicalPath;
            }
        });

        return app;
    }
    public static void HandleRecurringJobs(this IApplicationBuilder app, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
    {
        //recurringJobManager.AddOrUpdate(nameof(IJobService.SendSMSs), () => serviceProvider.GetService<IJobService>().SendSMSs(CancellationToken.None), "*/10 * * * *", TimeZoneInfo.Local);
        //recurringJobManager.AddOrUpdate(nameof(IJobService.SubmitTransferRequests), () => serviceProvider.GetService<IJobService>().SubmitTransferRequests(CancellationToken.None), "0 8-18 * * 0-4,6", TimeZoneInfo.Local);
        //recurringJobManager.AddOrUpdate(nameof(IJobService.CheckTransfersStatus), () => serviceProvider.GetService<IJobService>().CheckTransfersStatus(CancellationToken.None), "30 0-23 * * 0-4,6", TimeZoneInfo.Local);
        //recurringJobManager.AddOrUpdate(nameof(IJobService.RefreshFinnotechToken), () => serviceProvider.GetService<IJobService>().RefreshFinnotechToken(CancellationToken.None), "30 10 * * 6", TimeZoneInfo.Local);
        //recurringJobManager.AddOrUpdate(nameof(IJobService.AcceptPayableWithdrawRequests), () => serviceProvider.GetService<IJobService>().AcceptPayableWithdrawRequests(CancellationToken.None), Cron.Daily(8, 10), TimeZoneInfo.Local);
        //app.HandleRecurringJobs(recurringJobManager, serviceProvider);
    }

}
