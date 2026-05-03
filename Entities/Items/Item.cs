using Entities.Categories;
using Entities.Common;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;


namespace Entities.Items
{
    public class Item : BaseEntity<long>
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal PriceAmount { get; set; }
        public string[]? ImageUrls { get; set; }
        public bool IsActive { get; set; } = true;
        public long OrganizationId { get; set; }
        public long CategoryId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public Category Category { get; set; } = null!;
    }
    public class PostConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("menu_item");
            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.OrganizationId).HasColumnName("organization_id");
            builder.Property(p => p.CategoryId).HasColumnName("category_id");
            builder.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
            builder.Property(p => p.Code).HasColumnName("code").IsRequired().HasMaxLength(100);
            builder.Property(p => p.PriceAmount).HasColumnName("price_amount").HasColumnType("numeric(18,2)");
            builder.Property(p => p.ImageUrls).HasColumnName("image_urls");
            builder.Property(p => p.Description).HasColumnName("description");
            builder.Property(p => p.IsActive).HasColumnName("is_active");
            builder.Property(p => p.CreatedAt).HasColumnName("created_at");
            builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            builder.HasOne(p => p.Category).WithMany(c => c.Items).HasForeignKey(p => p.CategoryId);
        }
    }


}
