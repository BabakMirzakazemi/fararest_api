using Common.Markers;
using Common.Utilities.Extensions;
using Entities.Emails;
using Entities.Enums.Emails;

namespace Services.DTOs.Mails;

public class EmailReceiverDTO : IHaveCustomMapping
{
    public string Subject { get; set; } = null!;
    public EmailStatus Status { get; set; }
    public string StatusText => Status.ToDisplay();
    public DateTime CreatedDate { get; set; }
    public DateTime? SentDate { get; set; }
    public void CreateMappings(Profile profile)
    {
        profile.CreateMap<Email, EmailReceiverDTO>();
    }
}
