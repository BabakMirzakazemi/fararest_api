using Entities.Common;
using Entities.Emails;

namespace Entities.Documents;

public class EmailDocument : BaseDocumentEntity
{
    public int EmailSharedInformationId { get; set; }

    public EmailSharedInformation EmailSharedInformation { get; set; } = null!;
}
