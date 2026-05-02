using System.ComponentModel.DataAnnotations;

namespace Entities.Enums.Users;

public enum BankAccountStatus
{
    [Display(Name = "درحال بررسی")]
    Pending = 1,

    [Display(Name = "تایید شده")]
    Approved = 2,

    [Display(Name = "رد شده")]
    Deny = 3,
}
