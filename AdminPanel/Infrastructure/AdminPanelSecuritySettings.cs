namespace AdminPanel.Infrastructure;

public sealed class AdminPanelSecuritySettings
{
    public const string SectionName = "AdminPanelSecurity";
    public AdminPanelAuthenticationSettings Authentication { get; set; } = new();
    public OperationCatalogSecuritySettings OperationCatalog { get; set; } = new();
}

public sealed class AdminPanelAuthenticationSettings
{
    // Roles allowed to access AdminPanel after successful email/password authentication.
    public string[] AllowedRoles { get; set; } = ["Admin", "SuperAdmin"];
    public string LoginPath { get; set; } = "/Auth/Login";
    public string AccessDeniedPath { get; set; } = "/Auth/Login";
}

public sealed class OperationCatalogSecuritySettings
{
    // Master switch for reflective operation invocation UI.
    public bool Enabled { get; set; }
    // Explicit service contract allow-list; examples: IUserService, IRoleService
    public string[] AllowedServiceContracts { get; set; } = [];
    // Explicit operation allow-list; examples: IUserService.All, IUserService.AllByCursor
    public string[] AllowedOperationKeys { get; set; } = [];
    // Defensive max length for each raw parameter value in invoke form.
    public int MaxParameterLength { get; set; } = 4000;
}

public sealed record AdminPanelSignInRequest(string Email, string Password, bool RememberMe);

public sealed record AdminPanelSignInResult(bool Succeeded, string Message)
{
    public static AdminPanelSignInResult Success() => new(true, "Login successful.");
    public static AdminPanelSignInResult Failed(string message) => new(false, message);
}
