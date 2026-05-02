using Entities.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Documents.Configurations;

public class EmailDocumentConfiguration : IEntityTypeConfiguration<EmailDocument>
{
    public void Configure(EntityTypeBuilder<EmailDocument> builder)
    {
        
    }
}
