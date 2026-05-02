using Common.Configurations;
using System.ComponentModel;

namespace Services.DTOs.Admins.Create;

public class CreateAdminRequest
{
    [DisplayName(ApplicationPropertyPersianName.Password)]
    public string Password { get; set; } = null!;

    [DisplayName(ApplicationPropertyPersianName.RepeatPassword)]
    public string RepeatPassword { get; set; } = null!;

    [DisplayName(ApplicationPropertyPersianName.PhoneNumber)]
    public string PhoneNumber { get; set; } = null!;

    [DisplayName(ApplicationPropertyPersianName.NaturalNationalCode)]
    public string NationalCode { get; set; } = null!;

    [DisplayName(ApplicationPropertyPersianName.List)]
    public List<string> Roles { get; set; } = null!;
}
