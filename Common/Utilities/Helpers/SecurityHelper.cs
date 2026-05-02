using System.Security.Cryptography;
using System.Text;

namespace Common.Utilities.Helpers;

public static class SecurityHelper
{
    private static readonly byte[] DefaultSalt = [0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76];
    private const int LegacyIterationCount = 1000;
    private const string DefaultEncryptionKeyEnvName = "APP_DEFAULT_ENCRYPTION_KEY";
    private const string LegacyFallbackEncryptionKey = "pfmF0w6yAOlDgIZ7u4gMnxNerZPxhiq8";

    public static string GetSha256Hash(string input, string salt)
    {
        input = $"{salt}{input}{salt}";

        using (var sha256 = SHA256.Create())
        {
            var byteValue = Encoding.UTF8.GetBytes(input);
            var byteHash = sha256.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }
    }

    public static string Encrypt(this string clearText)
    {
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        var encryptionKey = ResolveDefaultEncryptionKey();
        using (Aes encryptor = Aes.Create())
        {
            ConfigureAesFromPassword(encryptor, encryptionKey);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
            }
            clearText = Convert.ToBase64String(ms.ToArray());
        }

        return clearText.Replace("/", "_").Replace("+", "-").Replace("=", "*");
    }

    public static string Decrypt(this string cipherText)
    {
        cipherText = cipherText.Replace("_", "/").Replace("-", "+").Replace("*", "=");
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        var encryptionKey = ResolveDefaultEncryptionKey();
        using (Aes encryptor = Aes.Create())
        {
            ConfigureAesFromPassword(encryptor, encryptionKey);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.Close();
            }
            cipherText = Encoding.Unicode.GetString(ms.ToArray());
        }
        return cipherText;
    }

    public static string Encrypt(this string clearText, string key)
    {
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            ConfigureAesFromPassword(encryptor, key);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
            }
            clearText = Convert.ToBase64String(ms.ToArray());
        }

        return clearText.Replace("/", "_").Replace("+", "-").Replace("=", "*");
    }

    public static string Decrypt(this string cipherText, string key)
    {
        cipherText = cipherText.Replace("_", "/").Replace("-", "+").Replace("*", "=");
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            ConfigureAesFromPassword(encryptor, key);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.Close();
            }
            cipherText = Encoding.Unicode.GetString(ms.ToArray());
        }
        return cipherText;
    }

    private static string ResolveDefaultEncryptionKey()
    {
        var keyFromEnv = Environment.GetEnvironmentVariable(DefaultEncryptionKeyEnvName);
        return string.IsNullOrWhiteSpace(keyFromEnv) ? LegacyFallbackEncryptionKey : keyFromEnv;
    }

    private static void ConfigureAesFromPassword(Aes encryptor, string key)
    {
        var keyMaterial = Rfc2898DeriveBytes.Pbkdf2(
            password: key,
            salt: DefaultSalt,
            iterations: LegacyIterationCount,
            hashAlgorithm: HashAlgorithmName.SHA1,
            outputLength: 48);

        try
        {
            encryptor.Key = keyMaterial[..32];
            encryptor.IV = keyMaterial[32..];
        }
        finally
        {
            CryptographicOperations.ZeroMemory(keyMaterial);
        }
    }
}
