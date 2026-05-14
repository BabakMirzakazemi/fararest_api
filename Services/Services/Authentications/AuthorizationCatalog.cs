namespace Services.Services.Authentications;

public static class AuthorizationCatalog
{
    public static readonly (string Key, string Name, string Category)[] Permissions =
    [
        ("home.view", "Home Dashboard View", "home"),
        ("menu.home", "Home Menu", "menu"),
        ("menu.accounts", "Accounts Menu", "menu"),
        ("menu.licenses", "Licenses Menu", "menu"),
        ("menu.payments", "Payments Menu", "menu"),
        ("menu.support", "Support Menu", "menu"),
        ("menu.crm", "CRM Menu", "menu"),
        ("menu.cashier", "Cashier Menu", "menu"),
        ("menu.manager-dashboard", "Manager-dashboard Menu", "menu"),
        ("menu.ai-assistant", "AI-assistant Menu", "menu"),
        ("menu.cashier-managment", "Cashier-managment Menu", "menu"),
        ("menu.kitchen", "Kitchen Menu", "menu"),
        ("menu.reservations", "Reservations Menu", "menu"),
        ("menu.menu", "Menu Menu", "menu"),
        ("menu.inventory", "Inventory Menu", "menu"),
        ("menu.employees", "Employees Menu", "menu"),
        ("menu.accounting", "Accounting Menu", "menu"),
        ("menu.sales-prediction", "Sales-prediction Menu", "menu"),
        ("menu.digital-menu", "Digital-menu Menu", "menu"),
        ("menu.modian", "modian Menu", "menu"),
        ("menu.settings", "Settings Menu", "menu"),
        ("accounts.view", "Accounts Read", "accounts"),
        ("accounts.create", "Accounts Create", "accounts"),
        ("accounts.update", "Accounts Update", "accounts"),
        ("accounts.delete", "Accounts Delete", "accounts"),
        ("licenses.view", "Licenses Read", "licenses"),
        ("licenses.create", "Licenses Create", "licenses"),
        ("licenses.update", "Licenses Update", "licenses"),
        ("licenses.delete", "Licenses Delete", "licenses"),
        ("payments.view", "Payments Read", "payments"),
        ("payments.create", "Payments Create", "payments"),
        ("payments.update", "Payments Update", "payments"),
        ("payments.delete", "Payments Delete", "payments"),
        ("crm.view", "CRM Read", "crm"),
        ("crm.create", "CRM Create", "crm"),
        ("crm.update", "CRM Update", "crm"),
        ("crm.delete", "CRM Delete", "crm"),
        ("support.view", "Support Read", "support"),
        ("support.create", "Support Create", "support"),
        ("support.update", "Support Update", "support"),
        ("support.delete", "Support Delete", "support"),
        ("admin.authz.manage", "Authorization Management", "admin")
    ];

    public static readonly string[] BasicPlanPermissions =
    [
        "home.view",
        "menu.home",
        "menu.accounts",
        "accounts.view"
    ];

    public static readonly string[] ProPlanPermissions =
    [
        ..BasicPlanPermissions,
        "menu.licenses",
        "licenses.view",
        "menu.payments",
        "payments.view"
    ];

    public static readonly string[] EnterprisePlanPermissions =
    [
        ..ProPlanPermissions,
        "menu.crm",
        "crm.view",
        "menu.support",
        "support.view"
    ];
}
