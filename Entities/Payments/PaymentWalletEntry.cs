using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Payments;

public class PaymentWalletEntry : BaseEntity<long>, IEntity
{
    public long WalletId { get; set; }
    public long OrganizationId { get; set; }
    public long OperationId { get; set; }
    public string EntrySide { get; set; } = string.Empty;
    public string EntryKind { get; set; } = string.Empty;
    public string SourceCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Description { get; set; }
    public string? Metadata { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class PaymentWalletEntryConfiguration : IEntityTypeConfiguration<PaymentWalletEntry>
{
    public void Configure(EntityTypeBuilder<PaymentWalletEntry> builder)
    {
        builder.ToTable("payments_wallet_entry");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.WalletId).HasColumnName("wallet_id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id");
        builder.Property(x => x.OperationId).HasColumnName("operation_id");
        builder.Property(x => x.EntrySide).HasColumnName("entry_side").HasMaxLength(20).IsRequired();
        builder.Property(x => x.EntryKind).HasColumnName("entry_kind").HasMaxLength(30).IsRequired();
        builder.Property(x => x.SourceCode).HasColumnName("source_code").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").HasColumnType("numeric(16,2)");
        builder.Property(x => x.BalanceBefore).HasColumnName("balance_before").HasColumnType("numeric(16,2)");
        builder.Property(x => x.BalanceAfter).HasColumnName("balance_after").HasColumnType("numeric(16,2)");
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.Metadata).HasColumnName("metadata").HasColumnType("jsonb");
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
    }
}
