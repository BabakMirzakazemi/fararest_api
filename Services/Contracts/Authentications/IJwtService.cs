using Services.DTOs.Accounts;

namespace Services.Contracts.Authentications;

public interface IJwtService
{
    Task<TokenDTO> GenerateAsync(User user);

    ClaimsPrincipal? Validate(string token);
}
