using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Support;

public class SupportTicket : BaseEntity<long>, IEntity
{
    public string TicketNo { get; set; } = string.Empty;
    public long OrganizationId { get; set; }
    public int RequesterUserId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int? AssignedToUserId { get; set; }
    public DateTimeOffset? FirstResponseAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public DateTimeOffset LastActivityAt { get; set; }
    public int ReopenCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class SupportTicketAttachment : BaseEntity<long>, IEntity
{
    public long TicketId { get; set; }
    public long? MessageId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public int? UploadedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class SupportTicketEvent : BaseEntity<long>, IEntity
{
    public long TicketId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public int? ActorUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class SupportTicketMessage : BaseEntity<long>, IEntity
{
    public long TicketId { get; set; }
    public int? AuthorUserId { get; set; }
    public string AuthorType { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class SupportTicketMetrics : IEntity
{
    public long? OrganizationId { get; set; }
    public long? OpenCount { get; set; }
    public long? InReviewCount { get; set; }
    public long? AnsweredCount { get; set; }
    public long? ClosedCount { get; set; }
    public long? TotalCount { get; set; }
}

public class SupportConfiguration :
    IEntityTypeConfiguration<SupportTicket>, IEntityTypeConfiguration<SupportTicketAttachment>,
    IEntityTypeConfiguration<SupportTicketEvent>, IEntityTypeConfiguration<SupportTicketMessage>,
    IEntityTypeConfiguration<SupportTicketMetrics>
{
    public void Configure(EntityTypeBuilder<SupportTicket> b)
    {
        b.ToTable("support_ticket");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.TicketNo).HasColumnName("ticket_no").HasMaxLength(24);
        b.Property(x => x.OrganizationId).HasColumnName("organization_id");
        b.Property(x => x.RequesterUserId).HasColumnName("requester_user_id");
        b.Property(x => x.SubjectCode).HasColumnName("subject_code").HasMaxLength(30);
        b.Property(x => x.Message).HasColumnName("message");
        b.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValueSql("'open'::character varying");
        b.Property(x => x.Priority).HasColumnName("priority").HasMaxLength(10).HasDefaultValueSql("'normal'::character varying");
        b.Property(x => x.AssignedToUserId).HasColumnName("assigned_to_user_id");
        b.Property(x => x.FirstResponseAt).HasColumnName("first_response_at");
        b.Property(x => x.ResolvedAt).HasColumnName("resolved_at");
        b.Property(x => x.ClosedAt).HasColumnName("closed_at");
        b.Property(x => x.LastActivityAt).HasColumnName("last_activity_at").HasDefaultValueSql("now()");
        b.Property(x => x.ReopenCount).HasColumnName("reopen_count");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

        b.HasIndex(x => x.TicketNo).IsUnique();
        b.HasIndex(x => new { x.OrganizationId, x.Status, x.LastActivityAt });
        b.HasIndex(x => new { x.RequesterUserId, x.CreatedAt });
        b.HasIndex(x => x.AssignedToUserId).HasFilter("(assigned_to_user_id IS NOT NULL)");
    }

    public void Configure(EntityTypeBuilder<SupportTicketAttachment> b)
    {
        b.ToTable("support_ticket_attachment");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.TicketId).HasColumnName("ticket_id");
        b.Property(x => x.MessageId).HasColumnName("message_id");
        b.Property(x => x.FileUrl).HasColumnName("file_url");
        b.Property(x => x.UploadedByUserId).HasColumnName("uploaded_by_user_id");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => x.TicketId);
        b.HasIndex(x => x.MessageId).HasFilter("(message_id IS NOT NULL)");
    }

    public void Configure(EntityTypeBuilder<SupportTicketEvent> b)
    {
        b.ToTable("support_ticket_event");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.TicketId).HasColumnName("ticket_id");
        b.Property(x => x.EventType).HasColumnName("event_type").HasMaxLength(30);
        b.Property(x => x.OldValue).HasColumnName("old_value");
        b.Property(x => x.NewValue).HasColumnName("new_value");
        b.Property(x => x.ActorUserId).HasColumnName("actor_user_id");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.TicketId, x.CreatedAt });
    }

    public void Configure(EntityTypeBuilder<SupportTicketMessage> b)
    {
        b.ToTable("support_ticket_message");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.TicketId).HasColumnName("ticket_id");
        b.Property(x => x.AuthorUserId).HasColumnName("author_user_id");
        b.Property(x => x.AuthorType).HasColumnName("author_type").HasMaxLength(12).HasDefaultValueSql("'requester'::character varying");
        b.Property(x => x.Body).HasColumnName("body");
        b.Property(x => x.IsInternal).HasColumnName("is_internal");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.TicketId, x.CreatedAt });
    }

    public void Configure(EntityTypeBuilder<SupportTicketMetrics> b)
    {
        b.ToView("support_ticket_metrics");
        b.HasNoKey();
        b.Property(x => x.OrganizationId).HasColumnName("organization_id");
        b.Property(x => x.OpenCount).HasColumnName("open_count");
        b.Property(x => x.InReviewCount).HasColumnName("in_review_count");
        b.Property(x => x.AnsweredCount).HasColumnName("answered_count");
        b.Property(x => x.ClosedCount).HasColumnName("closed_count");
        b.Property(x => x.TotalCount).HasColumnName("total_count");
    }
}
