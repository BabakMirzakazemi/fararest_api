namespace Services.DTOs.Accounts;

public class TokenDTO
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = string.Empty;

    public int ExpiresIn { get; set; }

    public TokenDTO(JwtSecurityToken securityToken)
    {
        AccessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
        TokenType = "Bearer";
        ExpiresIn = (int)(securityToken.ValidTo - DateTime.UtcNow).TotalSeconds;
    }
}
