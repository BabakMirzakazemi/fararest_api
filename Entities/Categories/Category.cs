using Entities.Common;
using Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Categories
{
    public class Category : BaseEntity<long>
    {
        public long OrganizationId { get; set; }
        public long? ParentCategoryId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string[]? ImageUrls { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        [ForeignKey(nameof(ParentCategoryId))]
        public Category? ParentCategory { get; set; }
        public ICollection<Category> ChildCategories { get; set; } = new List<Category>();
        public ICollection<Item> Items { get; set; } = new List<Item>();

    }

    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("menu_category");
            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.OrganizationId).HasColumnName("organization_id");
            builder.Property(p => p.ParentCategoryId).HasColumnName("parent_id");
            builder.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).HasColumnName("description");
            builder.Property(p => p.ImageUrls).HasColumnName("image_urls");
            builder.Property(p => p.IsActive).HasColumnName("is_active");
            builder.Property(p => p.CreatedAt).HasColumnName("created_at");
            builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            // Self-referencing relation
            builder
                .HasOne(c => c.ParentCategory)       // هر دسته یک والد دارد
                .WithMany(c => c.ChildCategories)    // و هر والد چند فرزند دارد
                .HasForeignKey(c => c.ParentCategoryId) // کلید خارجی
                .OnDelete(DeleteBehavior.Restrict);  // جلوگیری از cascade delete
        }
    }
}
