using Entities.Enums.Documents;

namespace Entities.Common;

public abstract class BaseDocumentEntity : BaseEntity
{
    public string FilePath { get; set; } = null!;

    public DocumentType DocumentType { get; set; }

    public DocumentStatus DocumentStatus { get; set; }
}
