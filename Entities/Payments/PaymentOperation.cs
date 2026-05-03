using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Payments;

public class PaymentOperation : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public Guid OperationUuid { get; set; }
    public string? IdempotencyKey { get; set; }
    public string OperationType { get; set; } = string.Empty;
    public string PurposeCode { get; set; } = string.Empty;
    public string PaymentMode { get; set; } = string.Empty;
    public string? Provider { get; set; }
    public string? ProviderReference { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal WalletUsedAmount { get; set; }
    public decimal GatewayChargedAmount { get; set; }
    public decimal FinalDebitAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Metadata { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class PaymentOperationConfiguration : IEntityTypeConfiguration<PaymentOperation>
{
    public void Configure(EntityTypeBuilder<PaymentOperation> builder)
    {
        builder.ToTable("payments_operation");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id");
        builder.Property(x => x.OperationUuid).HasColumnName("operation_uuid");
        builder.Property(x => x.IdempotencyKey).HasColumnName("idempotency_key").HasMaxLength(150);
        builder.Property(x => x.OperationType).HasColumnName("operation_type").HasMaxLength(50).IsRequired();
        builder.Property(x => x.PurposeCode).HasColumnName("purpose_code").HasMaxLength(100).IsRequired();
        builder.Property(x => x.PaymentMode).HasColumnName("payment_mode").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Provider).HasColumnName("provider").HasMaxLength(50);
        builder.Property(x => x.ProviderReference).HasColumnName("provider_reference").HasMaxLength(150);
        builder.Property(x => x.RequestedAmount).HasColumnName("requested_amount").HasColumnType("numeric(16,2)");
        builder.Property(x => x.WalletUsedAmount).HasColumnName("wallet_used_amount").HasColumnType("numeric(16,2)").HasDefaultValue(0m);
        builder.Property(x => x.GatewayChargedAmount).HasColumnName("gateway_charged_amount").HasColumnType("numeric(16,2)").HasDefaultValue(0m);
        builder.Property(x => x.FinalDebitAmount).HasColumnName("final_debit_amount").HasColumnType("numeric(16,2)").HasDefaultValue(0m);
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.Metadata).HasColumnName("metadata").HasColumnType("jsonb");
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.HasIndex(x => x.OperationUuid).IsUnique();
    }
}
