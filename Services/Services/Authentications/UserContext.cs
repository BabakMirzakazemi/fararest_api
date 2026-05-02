using Common.Markers;
using Services.Contracts.Authentications;

namespace Services.Services.Authentications;
public class UserContext : IUserContext, IScopedDependency
{
    public Guid UserId { get; set; }

    public Guid AccountId { get; set; }
}
