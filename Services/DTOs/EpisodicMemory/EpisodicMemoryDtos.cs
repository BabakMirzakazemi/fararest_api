using Entities.EpisodicMemory;
using Services.DTOs.Common;

namespace Services.DTOs.EpisodicMemory;

public sealed class RecordEpisodeRequest
{
    public EpisodeType Type { get; set; }
    public EpisodeImportance Importance { get; set; } = EpisodeImportance.Medium;
    public EpisodeSource Source { get; set; } = EpisodeSource.Agent;
    public EpisodeStatus? Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; }
    public string? ActorId { get; set; }
    public string? ActorName { get; set; }
    public string? CorrelationId { get; set; }
    public string? Environment { get; set; }
    public string? CommitSha { get; set; }
    public string? DeduplicationKey { get; set; }
    public string? MetadataJson { get; set; }
    public Guid? ParentEpisodeId { get; set; }
    public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
    public IReadOnlyList<EpisodeReferenceInput> References { get; set; } = Array.Empty<EpisodeReferenceInput>();
}

public sealed class SearchEpisodesRequest : PagingRequest
{
    public string? Query { get; set; }
    public IReadOnlyList<EpisodeType> Types { get; set; } = Array.Empty<EpisodeType>();
    public IReadOnlyList<EpisodeImportance> Importances { get; set; } = Array.Empty<EpisodeImportance>();
    public IReadOnlyList<EpisodeSource> Sources { get; set; } = Array.Empty<EpisodeSource>();
    public string? Environment { get; set; }
    public DateTimeOffset? OccurredFromUtc { get; set; }
    public DateTimeOffset? OccurredToUtc { get; set; }
    public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
    public EpisodeReferenceType? ReferenceType { get; set; }
    public string? ReferenceKey { get; set; }
    public string? CommitSha { get; set; }
    public string? CorrelationId { get; set; }
}

public sealed class GetRecentEpisodesRequest : PagingRequest
{
    public int Days { get; set; } = 30;
    public IReadOnlyList<EpisodeType> Types { get; set; } = Array.Empty<EpisodeType>();
}

public sealed class GetImportantEpisodesRequest : PagingRequest
{
    public EpisodeImportance MinimumImportance { get; set; } = EpisodeImportance.High;
    public int Days { get; set; } = 90;
}

public sealed class EpisodeDto
{
    public Guid Id { get; set; }
    public EpisodeType Type { get; set; }
    public EpisodeImportance Importance { get; set; }
    public EpisodeSource Source { get; set; }
    public EpisodeStatus? Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; }
    public DateTimeOffset RecordedAtUtc { get; set; }
    public string? ActorId { get; set; }
    public string? ActorName { get; set; }
    public string? CorrelationId { get; set; }
    public string? Environment { get; set; }
    public string? CommitSha { get; set; }
    public string? DeduplicationKey { get; set; }
    public string? MetadataJson { get; set; }
    public Guid? ParentEpisodeId { get; set; }
    public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
    public IReadOnlyList<EpisodeReferenceDto> References { get; set; } = Array.Empty<EpisodeReferenceDto>();
}

public sealed class EpisodeListItemDto
{
    public Guid Id { get; set; }
    public EpisodeType Type { get; set; }
    public EpisodeImportance Importance { get; set; }
    public EpisodeSource Source { get; set; }
    public EpisodeStatus? Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTimeOffset OccurredAtUtc { get; set; }
    public DateTimeOffset RecordedAtUtc { get; set; }
    public string? ActorName { get; set; }
    public string? Environment { get; set; }
    public string? CommitSha { get; set; }
    public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
}

public sealed class EpisodeReferenceInput
{
    public EpisodeReferenceType Type { get; set; }
    public string ReferenceKey { get; set; } = string.Empty;
    public string? ReferenceLabel { get; set; }
}

public sealed class EpisodeReferenceDto
{
    public EpisodeReferenceType Type { get; set; }
    public string ReferenceKey { get; set; } = string.Empty;
    public string? ReferenceLabel { get; set; }
}
