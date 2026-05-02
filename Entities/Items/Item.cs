using Entities.Categories;
using Entities.Common;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;


namespace Entities.Items
{
    public class Item : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public float Price { get; set; }
        public int CategoryId { get; set; }
        public Guid AuthorId { get; set; }
        public Category Category { get; set; } = null!;
    }
    public class PostConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).IsRequired();
            builder.HasOne(p => p.Category).WithMany(c => c.Items).HasForeignKey(p => p.CategoryId);
        }
    }


}
