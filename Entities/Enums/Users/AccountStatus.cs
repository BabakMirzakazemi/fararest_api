using System.ComponentModel.DataAnnotations;

namespace Entities.Enums.Users;

public enum AccountStatus
{
    [Display(Name = "نا مشخص")]
    UnKnown = 0,

    [Display(Name = "تایید شده")]
    Approved = 1,

    [Display(Name = "رد شده")]
    Deny = 2,

    [Display(Name = "درحال بررسی")]
    Pending = 3,

    [Display(Name = "در انتظار بررسی مدارک")]
    WaitingForReviewDocuments = 4,
}
