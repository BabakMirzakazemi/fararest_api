namespace Services.DTOs.AgentTools;

public sealed class AgentCapabilitiesDto
{
    public string ProjectName { get; set; } = string.Empty;
    public string OpenApiPath { get; set; } = string.Empty;
    public string SwaggerUiPath { get; set; } = string.Empty;
    public bool DirectDatabaseAccessRecommended { get; set; }
    public string DatabaseAccessRecommendation { get; set; } = string.Empty;
    public IReadOnlyList<string> RecommendedWorkflowSteps { get; set; } = Array.Empty<string>();
    public IReadOnlyList<AgentToolDescriptorDto> Tools { get; set; } = Array.Empty<AgentToolDescriptorDto>();
}

public sealed class AgentToolDescriptorDto
{
    public string ToolName { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public bool IsReadOnly { get; set; }
    public bool RecommendedForMcpAdapter { get; set; }
    public string Purpose { get; set; } = string.Empty;
}

public sealed class AgentDatabaseStatusDto
{
    public string Provider { get; set; } = string.Empty;
    public bool CanConnect { get; set; }
    public int AppliedMigrationCount { get; set; }
    public int PendingMigrationCount { get; set; }
    public string? LatestAppliedMigration { get; set; }
    public IReadOnlyList<string> PendingMigrations { get; set; } = Array.Empty<string>();
    public string? Error { get; set; }
}

public sealed class AgentMemoryStatusDto
{
    public long TotalEpisodes { get; set; }
    public int EpisodesRecordedLast7Days { get; set; }
    public int EpisodesRecordedLast30Days { get; set; }
    public Guid? LatestEpisodeId { get; set; }
    public string? LatestEpisodeTitle { get; set; }
    public DateTimeOffset? LatestRecordedAtUtc { get; set; }
    public bool SearchRecommended { get; set; }
    public bool RecordRecommendedAfterSignificantChange { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}
