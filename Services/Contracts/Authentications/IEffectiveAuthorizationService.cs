using Services.DTOs.Accounts.Authorization;

namespace Services.Contracts.Authentications;

public interface IEffectiveAuthorizationService
{
    Task<CurrentUserAuthorizationResponse> GetEffectiveAuthorizationAsync(Guid userId, CancellationToken cancellationToken);
    Task InvalidateUserAuthorizationAsync(Guid userId, CancellationToken cancellationToken);
}
