using Common.Configurations;
using Common.Utilities.Helpers;
using Data;
using Data.Database.SqlObjects;
using Entities.EpisodicMemory;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using Services.Contracts.EpisodicMemory;
using Services.DataInitializer;
using Services.DTOs.EpisodicMemory;
using System.Reflection;
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

    public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app, bool bootstrapMode = false)
    {
        Assert.NotNull(app, nameof(app));

        using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var hostEnvironment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

        dbContext.Database.Migrate();
        DatabaseSqlObjectsInstaller.ApplyAsync(dbContext).GetAwaiter().GetResult();

        var dataInitializers = scope.ServiceProvider.GetServices<IDataInitializer>().OrderBy(i => i.Order).ToList();
        Console.WriteLine($"[Seed] IDataInitializer count: {dataInitializers.Count}");
        foreach (var dataInitializer in dataInitializers)
        {
            Console.WriteLine($"[Seed] Running: {dataInitializer.GetType().FullName}");
            dataInitializer.InitializeData();
        }

        TryRecordDatabaseAutomationEpisodesAsync(scope.ServiceProvider, hostEnvironment, pendingMigrations, bootstrapMode).GetAwaiter().GetResult();

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

        app.UseMiddleware<RequestTelemetryMiddleware>();

        if (telemetry.Enabled && telemetry.EnablePrometheusScrapingEndpoint)
            app.UseOpenTelemetryPrometheusScrapingEndpoint(path: metricsPath);

        app.UseMiddleware<SimpleRateLimitMiddleware>();

        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live")
        });

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

    private static async Task TryRecordDatabaseAutomationEpisodesAsync(
        IServiceProvider serviceProvider,
        IHostEnvironment hostEnvironment,
        IReadOnlyCollection<string> pendingMigrations,
        bool bootstrapMode)
    {
        if (serviceProvider.GetService(typeof(IEpisodicMemoryAutomationService)) is not IEpisodicMemoryAutomationService automationService)
            return;

        var now = DateTimeOffset.UtcNow;
        var runtimeVersion = ResolveRuntimeVersion();
        var commitSha = ResolveCommitSha();
        var environmentName = hostEnvironment.EnvironmentName;
        var deploymentTags = new List<string>
        {
            "deployment",
            "startup",
            environmentName.ToLowerInvariant()
        };

        if (bootstrapMode)
            deploymentTags.Add("bootstrap");

        await automationService.TryRecordAsync(new RecordEpisodeRequest
        {
            Type = EpisodeType.DeploymentEvent,
            Importance = bootstrapMode ? EpisodeImportance.High : EpisodeImportance.Medium,
            Source = EpisodeSource.System,
            Status = EpisodeStatus.Informational,
            Title = bootstrapMode
                ? "Application bootstrap startup executed"
                : "Application startup executed",
            Summary = $"Environment={environmentName}; Version={runtimeVersion ?? "unknown"}; PendingMigrationsBeforeStart={pendingMigrations.Count}",
            Details = BuildDeploymentDetails(environmentName, runtimeVersion, commitSha, bootstrapMode, pendingMigrations, now),
            OccurredAtUtc = now,
            ActorName = nameof(ApplicationBuilderExtensions),
            Environment = environmentName,
            CommitSha = commitSha,
            DeduplicationKey = $"deployment|{environmentName.ToLowerInvariant()}|{runtimeVersion ?? "unknown"}|{commitSha ?? "no-commit"}|{now:yyyyMMdd}",
            Tags = deploymentTags,
            References =
            [
                new EpisodeReferenceInput
                {
                    Type = EpisodeReferenceType.Module,
                    ReferenceKey = "API.Program",
                    ReferenceLabel = "API startup"
                },
                new EpisodeReferenceInput
                {
                    Type = EpisodeReferenceType.Module,
                    ReferenceKey = "WebFramework.Configuration.ApplicationBuilderExtensions",
                    ReferenceLabel = nameof(ApplicationBuilderExtensions)
                }
            ]
        }, CancellationToken.None);

        if (pendingMigrations.Count == 0 && !bootstrapMode)
            return;

        var title = pendingMigrations.Count > 0
            ? "Applied EF Core migrations during application startup"
            : "Executed database bootstrap without pending migrations";
        var summary = pendingMigrations.Count > 0
            ? $"Applied {pendingMigrations.Count} migration(s): {string.Join(", ", pendingMigrations)}"
            : "Database bootstrap mode was executed and no pending migrations were found.";

        var references = pendingMigrations
            .Select(migrationId => new EpisodeReferenceInput
            {
                Type = EpisodeReferenceType.Migration,
                ReferenceKey = migrationId,
                ReferenceLabel = migrationId
            })
            .ToList();

        references.Add(new EpisodeReferenceInput
        {
            Type = EpisodeReferenceType.Module,
            ReferenceKey = "Data.ApplicationDbContext",
            ReferenceLabel = nameof(ApplicationDbContext)
        });

        await automationService.TryRecordAsync(new RecordEpisodeRequest
        {
            Type = pendingMigrations.Count > 0 ? EpisodeType.Migration : EpisodeType.DeploymentEvent,
            Importance = pendingMigrations.Count > 0 ? EpisodeImportance.High : EpisodeImportance.Medium,
            Source = EpisodeSource.System,
            Status = EpisodeStatus.Informational,
            Title = title,
            Summary = summary,
            Details = BuildDeploymentDetails(environmentName, runtimeVersion, commitSha, bootstrapMode, pendingMigrations, now),
            OccurredAtUtc = now,
            ActorName = nameof(ApplicationBuilderExtensions),
            Environment = environmentName,
            CommitSha = commitSha,
            DeduplicationKey = pendingMigrations.Count > 0
                ? $"migration|{environmentName.ToLowerInvariant()}|{runtimeVersion ?? "unknown"}|{commitSha ?? "no-commit"}|{string.Join("|", pendingMigrations)}"
                : $"bootstrap|{environmentName.ToLowerInvariant()}|{runtimeVersion ?? "unknown"}|{commitSha ?? "no-commit"}|{now:yyyyMMdd}",
            Tags = pendingMigrations.Count > 0
                ? ["migration", "startup", "database", environmentName.ToLowerInvariant()]
                : ["bootstrap", "database", environmentName.ToLowerInvariant()],
            References = references
        }, CancellationToken.None);
    }

    private static string BuildDeploymentDetails(
        string environmentName,
        string? runtimeVersion,
        string? commitSha,
        bool bootstrapMode,
        IReadOnlyCollection<string> pendingMigrations,
        DateTimeOffset occurredAtUtc)
    {
        return $"Environment: {environmentName}; Version: {runtimeVersion ?? "unknown"}; CommitSha: {commitSha ?? "unknown"}; BootstrapMode: {bootstrapMode}; PendingMigrations: {(pendingMigrations.Count == 0 ? "none" : string.Join(", ", pendingMigrations))}; OccurredAtUtc: {occurredAtUtc:O}";
    }

    private static string? ResolveRuntimeVersion()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null)
            return null;

        var informationalVersion = entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (!string.IsNullOrWhiteSpace(informationalVersion))
            return informationalVersion;

        return entryAssembly.GetName().Version?.ToString();
    }

    private static string? ResolveCommitSha()
    {
        var environmentVariableNames = new[]
        {
            "GIT_COMMIT",
            "SOURCE_VERSION",
            "BUILD_SOURCEVERSION",
            "RELEASE_COMMIT_SHA",
            "COMMIT_SHA"
        };

        foreach (var environmentVariableName in environmentVariableNames)
        {
            var value = Environment.GetEnvironmentVariable(environmentVariableName);
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }

        var informationalVersion = Assembly.GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        if (string.IsNullOrWhiteSpace(informationalVersion))
            return null;

        var plusIndex = informationalVersion.IndexOf('+');
        if (plusIndex < 0 || plusIndex == informationalVersion.Length - 1)
            return null;

        return informationalVersion[(plusIndex + 1)..].Trim();
    }
}
