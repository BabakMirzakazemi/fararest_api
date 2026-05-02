using Entities.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AdminPanel.Infrastructure;

public interface IAdminPanelAuthService
{
    Task<AdminPanelSignInResult> SignInWithEmailPasswordAsync(AdminPanelSignInRequest request, CancellationToken cancellationToken);
    Task SignOutAsync();
}

public sealed class AdminPanelAuthService(
    UserManager<User> userManager,
    IHttpContextAccessor httpContextAccessor,
    IOptions<AdminPanelSecuritySettings> securityOptions) : IAdminPanelAuthService
{
    private readonly AdminPanelAuthenticationSettings _authSettings = securityOptions.Value.Authentication;

    public async Task<AdminPanelSignInResult> SignInWithEmailPasswordAsync(AdminPanelSignInRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            return AdminPanelSignInResult.Failed("Email and password are required.");

        var normalizedEmail = email.ToUpperInvariant();
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToUpper() == normalizedEmail, cancellationToken);

        if (user is null)
            return AdminPanelSignInResult.Failed("Invalid credentials.");

        if (!user.IsActive)
            return AdminPanelSignInResult.Failed("Your account is inactive.");

        if (await userManager.IsLockedOutAsync(user))
            return AdminPanelSignInResult.Failed("Your account is locked.");

        var passwordOk = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordOk)
            return AdminPanelSignInResult.Failed("Invalid credentials.");

        var userRoles = await userManager.GetRolesAsync(user);
        if (!HasAdminAccess(userRoles, _authSettings.AllowedRoles))
            return AdminPanelSignInResult.Failed("You do not have permission to access AdminPanel.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = request.RememberMe,
            IssuedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = request.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : null
        };

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            return AdminPanelSignInResult.Failed("HttpContext is not available.");

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);

        return AdminPanelSignInResult.Success();
    }

    public async Task SignOutAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            return;

        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private static bool HasAdminAccess(IEnumerable<string> userRoles, IEnumerable<string>? allowedRoles)
    {
        var allowed = new HashSet<string>(
            allowedRoles?.Where(r => !string.IsNullOrWhiteSpace(r)) ?? [],
            StringComparer.OrdinalIgnoreCase);

        if (allowed.Count == 0)
            return false;

        return userRoles.Any(role => allowed.Contains(role));
    }
}
