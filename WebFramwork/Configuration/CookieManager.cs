using Microsoft.AspNetCore.Http;

namespace WebFramework.Configuration;

public static class CookieManager
{

    public enum CookieKeys
    {
        SiteJwtToken,
        CodeVerifier,
        ZPToken,
        ReferralCode,
        IsFirstLogin,
        IsLoginScriptCalled,
    }

    public enum ExpireTimeMode
    {
        Minute,
        Hour,
        Day
    }

    private static readonly Dictionary<CookieKeys, string> Keys = new Dictionary<CookieKeys, string>()
    {
        { CookieKeys.SiteJwtToken,"SITE_JWT_TOKEN"},
        { CookieKeys.CodeVerifier,"CODE_VERIFIER"},
        { CookieKeys.ZPToken,"ZARINPAL_TOKEN"},
        { CookieKeys.ReferralCode,"REFERRAL_CODE"},
        { CookieKeys.IsFirstLogin,"IS_FIRST_LOGIN"},
        { CookieKeys.IsLoginScriptCalled,"IS_LOGIN_SCRIPT_CALLED"},
    };

    public static string? Get(HttpContext context, CookieKeys key)
    {
        var cookieName = Keys[key];

        return context.Request.Cookies[cookieName];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expireTimeMode"></param>
    /// <param name="expireTime">expireTime of minutes</param>
    public static void Set(HttpContext context, CookieKeys key, string value, ExpireTimeMode? expireTimeMode = null, int? expireTime = null)
    {
        var cookieName = Keys[key];

        var option = new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Strict,
            Secure = false
        };

        if (expireTime.HasValue && expireTimeMode.HasValue)
        {
            option.Expires = expireTimeMode switch
            {
                ExpireTimeMode.Day => DateTime.Now.AddDays(expireTime.Value),
                ExpireTimeMode.Hour => DateTime.Now.AddHours(expireTime.Value),
                ExpireTimeMode.Minute => DateTime.Now.AddMinutes(expireTime.Value),
                _ => option.Expires
            };
        }

        context.Response.Cookies.Append(cookieName, value, option);
    }

    public static void Remove(HttpContext context, CookieKeys key)
    {
        var cookieName = Keys[key];

        context.Response.Cookies.Delete(cookieName);
    }

    public static void RemoveAllCookie(HttpContext context)
    {
        foreach (var key in Keys.Keys)
        {
            Remove(context, key);
        }
    }
}
