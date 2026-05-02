using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
namespace Common.Utilities.Extensions;

public static class StringExtensions
{
    public static bool HasValue(this string? value, bool ignoreWhiteSpace = true)
    {
        return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
    }

    public static int ToInt(this string value)
    {
        return Convert.ToInt32(value);
    }

    public static decimal ToDecimal(this string value)
    {
        return Convert.ToDecimal(value);
    }

    public static string ToNumeric(this int value)
    {
        return value.ToString("N0"); //"123,456"
    }

    public static string ToNumeric(this decimal value)
    {
        return value.ToString("N0");
    }

    public static string ToCurrency(this int value)
    {
        //fa-IR => current culture currency symbol => ریال
        //123456 => "123,123ریال"
        return value.ToString("C0");
    }

    public static string ToCurrency(this decimal value)
    {
        return value.ToString("C0");
    }

    public static string En2Fa(this string str)
    {
        return str.Replace("0", "۰")
            .Replace("1", "۱")
            .Replace("2", "۲")
            .Replace("3", "۳")
            .Replace("4", "۴")
            .Replace("5", "۵")
            .Replace("6", "۶")
            .Replace("7", "۷")
            .Replace("8", "۸")
            .Replace("9", "۹");
    }

    public static string Fa2En(this string str)
    {
        return str.Replace("۰", "0")
            .Replace("۱", "1")
            .Replace("۲", "2")
            .Replace("۳", "3")
            .Replace("۴", "4")
            .Replace("۵", "5")
            .Replace("۶", "6")
            .Replace("۷", "7")
            .Replace("۸", "8")
            .Replace("۹", "9")
            //iphone numeric
            .Replace("٠", "0")
            .Replace("١", "1")
            .Replace("٢", "2")
            .Replace("٣", "3")
            .Replace("٤", "4")
            .Replace("٥", "5")
            .Replace("٦", "6")
            .Replace("٧", "7")
            .Replace("٨", "8")
            .Replace("٩", "9");
    }

    public static string FixPersianChars(this string str)
    {
        return str.Replace("ﮎ", "ک")
            .Replace("ﮏ", "ک")
            .Replace("ﮐ", "ک")
            .Replace("ﮑ", "ک")
            .Replace("ك", "ک")
            .Replace("ي", "ی")
            .Replace(" ", " ")
            .Replace("‌", " ")
            .Replace("ھ", "ه");//.Replace("ئ", "ی");
    }

    public static string? CleanString(this string? str)
    {
        return str?.Trim().FixPersianChars().Fa2En().NullIfEmpty();
    }

    public static string? NullIfEmpty(this string? str)
    {
        return str?.Length == 0 ? null : str;
    }

    public static string ToBase64(this string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        return Convert.ToBase64String(bytes);
    }

    public static bool CheckValidCardNumber(this string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;

        int sum = 0;
        bool alternate = false;
        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            char[] nx = cardNumber.ToArray();
            int n = int.Parse(nx[i].ToString());

            if (alternate)
            {
                n *= 2;

                if (n > 9)
                {
                    n = n % 10 + 1;
                }
            }
            sum += n;
            alternate = !alternate;
        }

        return sum % 10 == 0 ? true : false;
    }
    public static string ReverseDateTime(this string str, string dateTimeSeparator = "-")
    {
        var splited = str.Split(dateTimeSeparator);
        return $"{splited[1]}-{splited[0]}";
    }

    public static string? GetEnumDisplayName(this Enum enumType)
    {
        return enumType.GetType().GetMember(enumType.ToString())
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>()
            ?.Name;
    }



    public static string? NormalizeMobile(this string? mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
            return null;

        mobile = mobile.Trim();

        // تبدیل ارقام فارسی و عربی به انگلیسی
        mobile = mobile
            .Replace('۰', '0').Replace('۱', '1').Replace('۲', '2').Replace('۳', '3').Replace('۴', '4')
            .Replace('۵', '5').Replace('۶', '6').Replace('۷', '7').Replace('۸', '8').Replace('۹', '9')
            .Replace('٠', '0').Replace('١', '1').Replace('٢', '2').Replace('٣', '3').Replace('٤', '4')
            .Replace('٥', '5').Replace('٦', '6').Replace('٧', '7').Replace('٨', '8').Replace('٩', '9');

        // حذف فاصله، خط تیره، پرانتز
        mobile = Regex.Replace(mobile, @"[\s\-\(\)]", "");

        // یکدست‌سازی به فرمت 09xxxxxxxxx
        if (mobile.StartsWith("+98"))
            mobile = "0" + mobile.Substring(3);
        else if (mobile.StartsWith("0098"))
            mobile = "0" + mobile.Substring(4);
        else if (mobile.StartsWith("98") && mobile.Length == 12)
            mobile = "0" + mobile.Substring(2);
        else if (mobile.StartsWith("9") && mobile.Length == 10)
            mobile = "0" + mobile;

        if (!Regex.IsMatch(mobile, @"^09\d{9}$"))
            throw new ArgumentException("Mobile format is invalid.");

        return mobile;
    }


}
