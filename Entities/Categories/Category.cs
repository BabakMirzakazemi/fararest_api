using Entities.Common;
using Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Categories
{
    public class Category : BaseEntity
    {
        public int? ParentCategoryId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [ForeignKey(nameof(ParentCategoryId))]
        public Category? ParentCategory { get; set; }
        public ICollection<Category> ChildCategories { get; set; } = new List<Category>();
        public ICollection<Item> Items { get; set; } = new List<Item>();

    }

    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            // Self-referencing relation
            builder
                .HasOne(c => c.ParentCategory)       // هر دسته یک والد دارد
                .WithMany(c => c.ChildCategories)    // و هر والد چند فرزند دارد
                .HasForeignKey(c => c.ParentCategoryId) // کلید خارجی
                .OnDelete(DeleteBehavior.Restrict);  // جلوگیری از cascade delete
        }
    }
}
