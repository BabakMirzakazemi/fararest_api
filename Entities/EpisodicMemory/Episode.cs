using Entities.Common;

namespace Entities.EpisodicMemory;

public sealed class Episode : BaseEntity<Guid>, IEntity
{
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
    public Episode? ParentEpisode { get; set; }
    public ICollection<Episode> FollowUpEpisodes { get; set; } = new List<Episode>();
    public ICollection<EpisodeTag> Tags { get; set; } = new List<EpisodeTag>();
    public ICollection<EpisodeReference> References { get; set; } = new List<EpisodeReference>();
}

public sealed class EpisodeTag : BaseEntity<long>, IEntity
{
    public Guid EpisodeId { get; set; }
    public string Tag { get; set; } = string.Empty;
    public Episode Episode { get; set; } = null!;
}

public sealed class EpisodeReference : BaseEntity<long>, IEntity
{
    public Guid EpisodeId { get; set; }
    public EpisodeReferenceType Type { get; set; }
    public string ReferenceKey { get; set; } = string.Empty;
    public string? ReferenceLabel { get; set; }
    public Episode Episode { get; set; } = null!;
}
