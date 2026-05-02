namespace Common.Configurations;

public class SiteSettings
{
    public string ElmahPath { get; set; } = string.Empty;
    public JwtSettings JwtSettings { get; set; } = new();
    public IdentitySettings IdentitySettings { get; set; } = new();
    public MailSettings MailSettings { get; set; } = new();
}

public class SeqSettings
{
    public string SeqUrl { get; set; } = string.Empty;

    public string SeqApiKey { get; set; } = string.Empty;
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Encryptkey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int NotBeforeMinutes { get; set; }
    public int ExpirationMinutes { get; set; }
}
public class IdentitySettings
{
    public bool PasswordRequireDigit { get; set; }
    public int PasswordRequiredLength { get; set; }
    public bool PasswordRequireNonAlphanumic { get; set; }
    public bool PasswordRequireUppercase { get; set; }
    public bool PasswordRequireLowercase { get; set; }
    public bool RequireUniqueEmail { get; set; }
}

public class MailSettings
{
    public string Mail { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string ActivationConfirmUrl { get; set; } = string.Empty;
}


