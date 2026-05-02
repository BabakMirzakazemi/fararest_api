using Entities.Common;
using Entities.Documents;
using Entities.Enums.Emails;

namespace Entities.Emails;

public class EmailSharedInformation : BaseEntity
{
    public string Subject { get; set; } = null!;

    public string HtmlBodyFilePath { get; set; } = null!;

    public string SenderName { get; set; } = null!;

    public EmailStatus Status { get; set; }

    public ICollection<Email> Emails { get; set; } = null!;

    public ICollection<EmailDocument>? Documents { get; set; }
}
