using Common.Configurations;
using Microsoft.AspNetCore.Identity;

namespace Services.DataInitializer;

public class RoleDataInitializer : IDataInitializer
{
    private readonly RoleManager<Role> _roleManager;

    public RoleDataInitializer(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public int Order => 1;

    public void InitializeData()
    {
        var roles = new List<Role>
        {
            new() { Name = RoleHelper.User, Description = RoleHelper.UserDescription },
            new() { Name = RoleHelper.Admin, Description = RoleHelper.AdminDescription },
            new() { Name = RoleHelper.SuperAdmin, Description = RoleHelper.SuperAdminDescription },
            new() { Name = RoleHelper.ThirdParty, Description = RoleHelper.ThirdPartyDescription },
            new() { Name = RoleHelper.FinanceManager, Description = RoleHelper.FinanceManagerDescription },
            new() { Name = RoleHelper.FinancePerformancer, Description = RoleHelper.FinancePerformancerDescription },
            new() { Name = RoleHelper.Marketing, Description = RoleHelper.MarketingDescription }
        };

        foreach (var role in roles)
        {
            var exists = _roleManager.RoleExistsAsync(role.Name!).GetAwaiter().GetResult();
            if (exists)
                continue;

            var result = _roleManager.CreateAsync(role).GetAwaiter().GetResult();
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new InvalidOperationException($"Role seed failed for '{role.Name}'. {errors}");
            }
        }
    }
}
