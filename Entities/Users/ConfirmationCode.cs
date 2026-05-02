using Entities.Common;

namespace Entities.Users;

public class ConfirmationCode : BaseEntity
{
    public string? LoginOtp { get; set; }

    public DateTime? LoginOtpExpirationDate { get; set; }

    public string? UpdatePasswordOtp { get; set; }

    public DateTime? UpdatePasswordOtpExpirationDate { get; set; }

    public string? CurrentPhoneNumberOtp { get; set; }

    public DateTime? CurrentPhoneNumberOtpExpirationDate { get; set; }

    public string? NewPhoneNumberOtp { get; set; }

    public DateTime? NewPhoneNumberOtpExpirationDate { get; set; }

    public string? CurrentEmailOtp { get; set; }

    public DateTime? CurrentEmailOtpExpirationDate { get; set; }

    public string? NewEmailOtp { get; set; }

    public DateTime? NewEmailOtpExpirationDate { get; set; }

    public User User { get; set; } = null!;
}
