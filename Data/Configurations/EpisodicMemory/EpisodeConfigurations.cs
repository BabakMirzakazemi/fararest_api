using Entities.EpisodicMemory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations.EpisodicMemory;

public sealed class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder.ToTable("memory_episode");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Type).HasColumnName("type").HasConversion<int>();
        builder.Property(x => x.Importance).HasColumnName("importance").HasConversion<int>();
        builder.Property(x => x.Source).HasColumnName("source").HasConversion<int>();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<int?>();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Summary).HasColumnName("summary").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Details).HasColumnName("details");
        builder.Property(x => x.OccurredAtUtc).HasColumnName("occurred_at_utc");
        builder.Property(x => x.RecordedAtUtc).HasColumnName("recorded_at_utc");
        builder.Property(x => x.ActorId).HasColumnName("actor_id").HasMaxLength(128);
        builder.Property(x => x.ActorName).HasColumnName("actor_name").HasMaxLength(200);
        builder.Property(x => x.CorrelationId).HasColumnName("correlation_id").HasMaxLength(128);
        builder.Property(x => x.Environment).HasColumnName("environment").HasMaxLength(64);
        builder.Property(x => x.CommitSha).HasColumnName("commit_sha").HasMaxLength(64);
        builder.Property(x => x.DeduplicationKey).HasColumnName("deduplication_key").HasMaxLength(256);
        builder.Property(x => x.MetadataJson).HasColumnName("metadata_json");
        builder.Property(x => x.ParentEpisodeId).HasColumnName("parent_episode_id");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");

        builder.HasIndex(x => x.OccurredAtUtc);
        builder.HasIndex(x => new { x.Type, x.OccurredAtUtc });
        builder.HasIndex(x => new { x.Importance, x.OccurredAtUtc });
        builder.HasIndex(x => x.DeduplicationKey);
        builder.HasIndex(x => x.CommitSha);
        builder.HasIndex(x => x.CorrelationId);

        builder.HasOne(x => x.ParentEpisode)
            .WithMany(x => x.FollowUpEpisodes)
            .HasForeignKey(x => x.ParentEpisodeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class EpisodeTagConfiguration : IEntityTypeConfiguration<EpisodeTag>
{
    public void Configure(EntityTypeBuilder<EpisodeTag> builder)
    {
        builder.ToTable("memory_episode_tag");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.EpisodeId).HasColumnName("episode_id");
        builder.Property(x => x.Tag).HasColumnName("tag").HasMaxLength(64).IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");

        builder.HasIndex(x => x.Tag);
        builder.HasIndex(x => new { x.EpisodeId, x.Tag }).IsUnique();

        builder.HasOne(x => x.Episode)
            .WithMany(x => x.Tags)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class EpisodeReferenceConfiguration : IEntityTypeConfiguration<EpisodeReference>
{
    public void Configure(EntityTypeBuilder<EpisodeReference> builder)
    {
        builder.ToTable("memory_episode_reference");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.EpisodeId).HasColumnName("episode_id");
        builder.Property(x => x.Type).HasColumnName("type").HasConversion<int>();
        builder.Property(x => x.ReferenceKey).HasColumnName("reference_key").HasMaxLength(256).IsRequired();
        builder.Property(x => x.ReferenceLabel).HasColumnName("reference_label").HasMaxLength(256);
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");

        builder.HasIndex(x => new { x.Type, x.ReferenceKey });
        builder.HasIndex(x => new { x.EpisodeId, x.Type, x.ReferenceKey }).IsUnique();

        builder.HasOne(x => x.Episode)
            .WithMany(x => x.References)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
