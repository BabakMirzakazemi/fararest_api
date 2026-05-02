using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Common.Utilities.Helpers;

public static class Assert
{
    public static void NotNull<T>(T obj, string name, string? message = null)
        where T : class
    {
        if (obj is null)
            throw new ArgumentNullException($"{name} : {typeof(T)}", message);
    }

    public static void NotNull<T>(T? obj, string name, string? message = null)
        where T : struct
    {
        if (!obj.HasValue)
            throw new ArgumentNullException($"{name} : {typeof(T)}", message);

    }

    public static void NotEmpty<T>(T obj, string name, string? message = null, T? defaultValue = default)
        where T : class
    {
        if (obj == defaultValue
            || obj is string str && string.IsNullOrWhiteSpace(str)
            || obj is IEnumerable list && !list.Cast<object>().Any())
        {
            throw new ArgumentException("Argument is empty : " + message, $"{name} : {typeof(T)}");
        }
    }
}

public class RegularExpression
{
    //public const string Email = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"
    public const string Email = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";
    public const string Mobile = @"^09([01239])\d{8}";
    public const string WebSite = @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";
    public const string NationalCode = @"^[0-9]{10}$";
    public const string Money = @"^\$?(\d{1,3},?(\d{3},?)*\d{3}(.\d{0,3})?|\d{1,3}(.\d{2})?)$";
    public const string IntegerNumber = @"^\d+";
    public const string FloatNumber = @"^\d+.?\d{0,2}$";
    public const string CompanyWebsitePostFix = "^[a-zA-Z0-9-]+$";
    public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d$@$!%*#?&]{5,}$";
    public const string Sheba = @"^(?:IR)(?=.{24}$)[0-9]*$";
    public const string PersianLetter = @"^[\u0600-\u06FF\s]+$";
    //public const string Monry = @"\d+(?:,\d{1,2})?";
}

public static class CodeGenerator
{
    private static readonly Random Random = new Random();

    public static string CodeVerifier => CreateCodeVerifier();

    public static string GenerateRandomNumber(int length)
    {
        const string chars = "0123456789";

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public static string GenerateRandomString(int length)
    {
        const string letters = "abcdefghijklmnopqrstuvwxyz";

        return new string(Enumerable.Repeat(letters, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    private static string CreateCodeVerifier()
    {
        const string chars = "abcdef0123456789";
        var nonce = new char[64];
        for (int i = 0; i < nonce.Length; i++)
            nonce[i] = chars[Random.Next(chars.Length)];

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(new string(nonce))).Replace("/", "_").Replace("+", "-").Replace("=", "");
    }

    public static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        return Convert.ToBase64String(hash).Replace('+', '-').Replace('/', '_').Replace("=", "");
    }
}
