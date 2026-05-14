namespace Services.Services.Authentications;

public static class AuthorizationCatalog
{
    public static readonly (string Key, string Name,string PersianName, string Category)[] Permissions =
    [
        ("home.view", "Home Dashboard View", " مشاهده خانه", "home"),
        ("menu.home", "Home Menu","خانه", "menu"),
        ("menu.accounts", "Accounts Menu","پروفایل", "menu"),
        ("menu.licenses", "Licenses Menu","لایسنس ها", "menu"),
        ("menu.payments", "Payments Menu","پرداخت ها", "menu"),
        ("menu.support", "Support Menu","پشتیبانی", "menu"),
        ("menu.crm", "CRM Menu","باشگاه مشتریان", "menu"),
        ("menu.cashier", "Cashier Menu","صندوقداری", "menu"),
        ("menu.manager-dashboard", "Manager-dashboard Menu","داشبورد مدیریت", "menu"),
        ("menu.ai-assistant", "AI-assistant Menu","دستیار هوش مصنوعی", "menu"),
        ("menu.cashier-managment", "Cashier-managment Menu","مدیریت صندوق ها", "menu"),
        ("menu.kitchen", "Kitchen Menu","آشپزخانه", "menu"),
        ("menu.reservations", "Reservations Menu","رزرواسیون", "menu"),
        ("menu.menu", "Menu Menu","مدیریت منو", "menu"),
        ("menu.inventory", "Inventory Menu","انبارداری", "menu"),
        ("menu.employees", "Employees Menu","کارمندان", "menu"),
        ("menu.accounting", "Accounting Menu","حسابداری", "menu"),
        ("menu.sales-prediction", "Sales-prediction Menu","پیش بینی فروش", "menu"),
        ("menu.digital-menu", "Digital-menu Menu","منوی دیجیتال", "menu"),
        ("menu.modian", "modian Menu","سامانه مودیان", "menu"),
        ("menu.settings", "Settings Menu","تنظیمات", "menu"),
        ("accounts.view", "Accounts Read","مشاهده پروفایل", "accounts"),
        ("accounts.create", "Accounts Create","ایجاد پروفایل", "accounts"),
        ("accounts.update", "Accounts Update","بروزرسانی پروفایل", "accounts"),
        ("accounts.delete", "Accounts Delete","حذف پروفایل", "accounts"),
        ("licenses.view", "Licenses Read","مشاهده لایسنس ها", "licenses"),
        ("licenses.create", "Licenses Create","ایجاد لایسنس", "licenses"),
        ("licenses.update", "Licenses Update","بروزرسانی لایسنس", "licenses"),
        ("licenses.delete", "Licenses Delete","حذف لایسنس", "licenses"),
        ("payments.view", "Payments Read","مشاهده پرداخت ها", "payments"),
        ("payments.create", "Payments Create","ایجاد پرداخت", "payments"),
        ("payments.update", "Payments Update","بروزرسانی پرداخت", "payments"),
        ("payments.delete", "Payments Delete","حذف پرداخت", "payments"),
        ("crm.view", "CRM Read","مشاهده باشگاه مشتریان", "crm"),
        ("crm.create", "CRM Create","ایجاد باشگاه مشتریان", "crm"),
        ("crm.update", "CRM Update","بروزرسانی باشگاه مشتریان", "crm"),
        ("crm.delete", "CRM Delete","حذف باشگاه مشتریان", "crm"),
        ("support.view", "Support Read","مشاهده پشتیبانی", "support"),
        ("support.create", "Support Create","ایجاد پشتیبانی", "support"),
        ("support.update", "Support Update","بروزرسانی پشتیبانی", "support"),
        ("support.delete", "Support Delete","حذف پشتیبانی", "support"),
        ("admin.authz.manage", "Authorization Management","مدیریت احرازهویت و دسترسی ها", "admin")
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
