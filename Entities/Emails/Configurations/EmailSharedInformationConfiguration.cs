using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Emails.Configurations;

public class EmailSharedInformationConfiguration : IEntityTypeConfiguration<EmailSharedInformation>
{
    public void Configure(EntityTypeBuilder<EmailSharedInformation> builder)
    {
        
    }
}
