using System.ComponentModel.DataAnnotations;

namespace Entities.Enums.Documents;

public enum DocumentStatus
{
    [Display(Name = "فعال")]
    Active = 1,

    [Display(Name = "غیرفعال")]
    DeActive = 2
}