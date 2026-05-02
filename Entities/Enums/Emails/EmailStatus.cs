using System.ComponentModel.DataAnnotations;

namespace Entities.Enums.Emails;

public enum EmailStatus
{
    [Display(Name = "در انتظار ارسال")]
    Pending = 1,

    [Display(Name = "ارسال شده")]
    Succeeded = 2,

    [Display(Name = "خطا در ارسال")]
    Failed = 3,

    [Display(Name = "لغو شده")]
    Cancelled = 4
}
