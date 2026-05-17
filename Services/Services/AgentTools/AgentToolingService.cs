using Common.Markers;
using Entities.EpisodicMemory;
using Microsoft.EntityFrameworkCore;
using Services.Contracts.AgentTools;
using Services.Contracts.Repositories;
using Services.DTOs.AgentTools;

namespace Services.Services.AgentTools;

public sealed class AgentToolingService(
    DbContext dbContext,
    IRepository<Episode> episodeRepository) : IAgentToolingService, IScopedDependency
{
    public Task<AgentCapabilitiesDto> GetCapabilitiesAsync(CancellationToken cancellationToken)
    {
        var tools = new List<AgentToolDescriptorDto>
        {
            new()
            {
                ToolName = "episodes.search",
                HttpMethod = "POST",
                Route = "/api/admin/v1/Episodes/SearchAsync",
                IsReadOnly = true,
                RecommendedForMcpAdapter = true,
                Purpose = "Searches episodic memory by text, tags, references, type, importance, environment, and time window."
            },
            new()
            {
                ToolName = "episodes.get",
                HttpMethod = "GET",
                Route = "/api/admin/v1/Episodes/GetAsync",
                IsReadOnly = true,
                RecommendedForMcpAdapter = true,
                Purpose = "Loads one episode with full details, tags, and references."
            },
            new()
            {
                ToolName = "episodes.recent",
                HttpMethod = "GET",
                Route = "/api/admin/v1/Episodes/RecentAsync",
                IsReadOnly = true,
                RecommendedForMcpAdapter = true,
                Purpose = "Returns recent episodes to help agents inspect recent project history before making changes."
            },
            new()
            {
                ToolName = "episodes.record",
                HttpMethod = "POST",
                Route = "/api/admin/v1/Episodes/RecordAsync",
                IsReadOnly = false,
                RecommendedForMcpAdapter = true,
                Purpose = "Records a durable project event, decision, migration, incident, or implementation outcome."
            },
            new()
            {
                ToolName = "agent.status.database",
                HttpMethod = "GET",
                Route = "/api/admin/v1/AgentTools/DatabaseStatusAsync",
                IsReadOnly = true,
                RecommendedForMcpAdapter = true,
                Purpose = "Returns database connectivity and migration status without giving direct SQL access."
            },
            new()
            {
                ToolName = "agent.status.memory",
                HttpMethod = "GET",
                Route = "/api/admin/v1/AgentTools/MemoryStatusAsync",
                IsReadOnly = true,
                RecommendedForMcpAdapter = true,
                Purpose = "Returns episodic memory coverage and freshness so agents can decide when to search or record."
            },
            new()
            {
                ToolName = "episodes.evaluate_search",
                HttpMethod = "POST",
                Route = "/api/admin/v1/Episodes/EvaluateSearchAsync",
                IsReadOnly = true,
                RecommendedForMcpAdapter = false,
                Purpose = "Evaluates retrieval quality for a known query and expected episode set using recall, precision, and reciprocal rank."
            }
        };

        var result = new AgentCapabilitiesDto
        {
            ProjectName = "fararest_api",
            OpenApiPath = "/swagger/v1/swagger.json",
            SwaggerUiPath = "/swagger",
            DirectDatabaseAccessRecommended = false,
            DatabaseAccessRecommendation = "Use the read-only database status tool and episodic memory APIs first. Direct PostgreSQL MCP access is intentionally not recommended in the current project state.",
            RecommendedWorkflowSteps =
            [
                "Check database and memory status before sensitive changes.",
                "Search episodic memory before migrations, auth/security edits, and repeated fixes.",
                "Use hybrid-ranked search when query intent is fuzzy or multi-signal.",
                "Record a new episode after a significant decision, fix, failure, migration, incident, or deployment event.",
                "Use search evaluation when tuning retrieval quality for repeated workflows."
            ],
            Tools = tools
        };

        return Task.FromResult(result);
    }

    public async Task<AgentDatabaseStatusDto> GetDatabaseStatusAsync(CancellationToken cancellationToken)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            var appliedMigrations = (await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).ToArray();
            var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToArray();

            return new AgentDatabaseStatusDto
            {
                Provider = dbContext.Database.ProviderName ?? "unknown",
                CanConnect = canConnect,
                AppliedMigrationCount = appliedMigrations.Length,
                PendingMigrationCount = pendingMigrations.Length,
                LatestAppliedMigration = appliedMigrations.LastOrDefault(),
                PendingMigrations = pendingMigrations
            };
        }
        catch (Exception ex)
        {
            return new AgentDatabaseStatusDto
            {
                Provider = dbContext.Database.ProviderName ?? "unknown",
                CanConnect = false,
                Error = $"{ex.GetType().Name}: {ex.Message}"
            };
        }
    }

    public async Task<AgentMemoryStatusDto> GetMemoryStatusAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var last7Days = now.AddDays(-7);
        var last30Days = now.AddDays(-30);

        var totalEpisodes = await episodeRepository.TableNoTracking
            .TagWith("AgentTools.MemoryStatus.Total")
            .LongCountAsync(cancellationToken);

        var episodesRecordedLast7Days = await episodeRepository.TableNoTracking
            .TagWith("AgentTools.MemoryStatus.Last7Days")
            .CountAsync(x => x.RecordedAtUtc >= last7Days, cancellationToken);

        var episodesRecordedLast30Days = await episodeRepository.TableNoTracking
            .TagWith("AgentTools.MemoryStatus.Last30Days")
            .CountAsync(x => x.RecordedAtUtc >= last30Days, cancellationToken);

        var latestEpisode = await episodeRepository.TableNoTracking
            .TagWith("AgentTools.MemoryStatus.Latest")
            .OrderByDescending(x => x.RecordedAtUtc)
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.RecordedAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        var recommendation = BuildRecommendation(totalEpisodes, latestEpisode?.RecordedAtUtc, episodesRecordedLast30Days);

        return new AgentMemoryStatusDto
        {
            TotalEpisodes = totalEpisodes,
            EpisodesRecordedLast7Days = episodesRecordedLast7Days,
            EpisodesRecordedLast30Days = episodesRecordedLast30Days,
            LatestEpisodeId = latestEpisode?.Id,
            LatestEpisodeTitle = latestEpisode?.Title,
            LatestRecordedAtUtc = latestEpisode?.RecordedAtUtc,
            SearchRecommended = totalEpisodes > 0,
            RecordRecommendedAfterSignificantChange = true,
            Recommendation = recommendation
        };
    }

    private static string BuildRecommendation(long totalEpisodes, DateTimeOffset? latestRecordedAtUtc, int episodesRecordedLast30Days)
    {
        if (totalEpisodes == 0)
            return "Episodic memory is empty. Seed the system with recent decisions, migrations, incidents, and successful implementations before relying on retrieval.";

        if (!latestRecordedAtUtc.HasValue)
            return "Search episodic memory before sensitive work and record fresh episodes after significant outcomes.";

        if (episodesRecordedLast30Days == 0 || latestRecordedAtUtc.Value < DateTimeOffset.UtcNow.AddDays(-30))
            return "Memory exists but looks stale. Search first, then record recent project events so retrieval stays useful.";

        return "Memory is active. Search before sensitive changes and record outcomes after important work.";
    }
}
