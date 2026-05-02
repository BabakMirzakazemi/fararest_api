namespace Common.Utilities.Helpers;

public static class RegexHelper
{
    //public const string Email = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"
    public const string Email = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";
    public const string Mobile = @"^09([01239])\d{8}";
    public const string WebSite = @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";
    public const string NationalCode = @"(\S)+";
    public const string Money = @"^\$?(\d{1,3},?(\d{3},?)*\d{3}(.\d{0,3})?|\d{1,3}(.\d{2})?)$";
    public const string Integer_Number = @"^\d+";
    public const string Flaot_Number = @"^\d+.?\d{0,2}$";
    public const string CompanyWebsite_PostFix = "^[a-zA-Z0-9-]+$";
    public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d$@$!%*#?&.]{8,}$";
}
