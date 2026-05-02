using Entities.Common;
using Entities.Enums.Emails;

namespace Entities.Emails;

public class Email : BaseEntity<long>
{
    public int EmailSharedInformationId { get; set; }

    public string Receiver { get; set; } = null!;

    public int RetryCount { get; set; }

    public DateTime? SentDate { get; set; }

    public EmailStatus Status { get; set; } = EmailStatus.Pending;

    public EmailSharedInformation EmailSharedInformation { get; set; } = null!;
}
