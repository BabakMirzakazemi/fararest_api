using Common.Configurations;
using Common.Markers;
using Microsoft.AspNetCore.Identity;
using Services.Contracts.Authentications;
using Services.DTOs.Accounts;

namespace Services.Services.Authentications;

public class JwtService : IJwtService, IScopedDependency
{
    private readonly SiteSettings _siteSetting;
    private readonly SignInManager<User> _signInManager;

    public JwtService(IOptionsSnapshot<SiteSettings> settings, SignInManager<User> signInManager)
    {
        _siteSetting = settings.Value;
        _signInManager = signInManager;
    }

    public async Task<TokenDTO> GenerateAsync(User user)
    {
        var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey); // longer that 16 character
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

        var encryptionKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.Encryptkey); //must be 16 character
        var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionKey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

        var claims = await GetClaimsAsync(user);
        var utcNow = DateTime.UtcNow;
        // ExpirationMinutes must be interpreted as minutes (not days).
        var expirationMinutes = _siteSetting.JwtSettings.ExpirationMinutes > 0
            ? _siteSetting.JwtSettings.ExpirationMinutes
            : 1;

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _siteSetting.JwtSettings.Issuer,
            Audience = _siteSetting.JwtSettings.Audience,
            IssuedAt = utcNow,
            NotBefore = utcNow.AddMinutes(_siteSetting.JwtSettings.NotBeforeMinutes),
            Expires = utcNow.AddMinutes(expirationMinutes),
            SigningCredentials = signingCredentials,
            EncryptingCredentials = encryptingCredentials,
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);

        return new TokenDTO(securityToken);
    }

    public ClaimsPrincipal? Validate(string token)
    {
        var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey); // longer that 16 character
        var issuerSigningKey = new SymmetricSecurityKey(secretKey);

        var encryptionKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.Encryptkey); //must be 16 character
        var tokenDecryptionKey = new SymmetricSecurityKey(encryptionKey);

        var parameters = new TokenValidationParameters()
        {
            ValidAudience = _siteSetting.JwtSettings.Audience,
            ValidIssuer = _siteSetting.JwtSettings.Issuer,
            ValidateIssuer = true,
            ValidateAudience = true,
            RequireExpirationTime = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true, // Ensure the token hasn't expired
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            TokenDecryptionKey = tokenDecryptionKey,
            ClockSkew = TimeSpan.Zero // No tolerance for clock skew
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        if (tokenHandler.CanReadToken(token))
        {
            return tokenHandler.ValidateToken(token, parameters, out SecurityToken securityToken);
        }
        return null;
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(User user)
    {
        var claimsPrincipal = await _signInManager.ClaimsFactory.CreateAsync(user);
        return claimsPrincipal.Claims;
    }

}
