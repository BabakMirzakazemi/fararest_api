using Entities.Accounts;
using Entities.Licenses;
using Microsoft.AspNetCore.Identity;
using Services.Contracts.Repositories;
using Services.Services.Authentications;

namespace Services.DataInitializer;

public class AuthorizationDataInitializer(
    RoleManager<Role> roleManager,
    IRepository<AccountPermission> permissionRepository,
    IRepository<AccountRolePermission> rolePermissionRepository,
    IRepository<AccountPlanPermission> planPermissionRepository,
    IRepository<AccountUserPlanSubscription> userPlanSubscriptionRepository,
    IRepository<LicensePlan> planRepository) : IDataInitializer
{
    public int Order => 3;

    public void InitializeData()
    {
        var now = DateTimeOffset.UtcNow;
        var allPermissions = new List<(string Key, string Name,string PersianName, string Category)>(AuthorizationCatalog.Permissions.Length);
        var seenPermissionKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var permission in AuthorizationCatalog.Permissions)
        {
            if (!seenPermissionKeys.Add(permission.Key))
                continue;

            allPermissions.Add(permission);
        }

        var permissionByKey = permissionRepository.Table.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
        foreach (var item in allPermissions)
        {
            if (permissionByKey.TryGetValue(item.Key, out var existing))
            {
                existing.Name = item.Name;
                existing.PersianName = item.PersianName;
                existing.Category = item.Category;
                existing.IsActive = true;
                existing.UpdatedAt = now;
                continue;
            }

            var row = new AccountPermission
            {
                Key = item.Key,
                Name = item.Name,
                PersianName = item.PersianName,
                Category = item.Category,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            permissionRepository.Add(row, saveNow: false);
            permissionByKey[item.Key] = row;
        }

        EnsurePlan("BASIC", "Basic", 1, now);
        EnsurePlan("PRO", "Pro", 2, now);
        EnsurePlan("ENTERPRISE", "Enterprise", 3, now);
        permissionRepository.SaveChanges();
        planRepository.SaveChanges();

        MapRolePermissions(RoleHelper.User, ["home.view", "menu.home", "menu.accounts", "accounts.view"], now);
        MapRolePermissions(RoleHelper.Admin, ["home.view", "menu.home", "menu.accounts", "accounts.view", "accounts.create", "accounts.update", "menu.support", "support.view"], now);
        MapRolePermissions(RoleHelper.SuperAdmin, allPermissions.Select(x => x.Key).ToArray(), now);

        MapPlanPermissions("BASIC", AuthorizationCatalog.BasicPlanPermissions, now);
        MapPlanPermissions("PRO", AuthorizationCatalog.ProPlanPermissions, now);
        MapPlanPermissions("ENTERPRISE", AuthorizationCatalog.EnterprisePlanPermissions, now);

        EnsureSeedUserPlan("ENTERPRISE", now);
        rolePermissionRepository.SaveChanges();
        planPermissionRepository.SaveChanges();
        userPlanSubscriptionRepository.SaveChanges();
    }

    private void EnsurePlan(string code, string name, short tier, DateTimeOffset now)
    {
        var plan = planRepository.Table.FirstOrDefault(x => x.Code == code);
        if (plan == null)
        {
            planRepository.Add(new LicensePlan
            {
                Code = code,
                Name = name,
                Tier = tier,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            }, saveNow: false);
            return;
        }

        plan.Name = name;
        plan.Tier = tier;
        plan.IsActive = true;
        plan.UpdatedAt = now;
    }

    private void MapRolePermissions(string roleName, IReadOnlyCollection<string> permissionKeys, DateTimeOffset now)
    {
        var role = roleManager.FindByNameAsync(roleName).GetAwaiter().GetResult();
        if (role == null)
            return;

        var permissionIds = permissionRepository.TableNoTracking
            .Where(x => permissionKeys.Contains(x.Key))
            .Select(x => x.Id)
            .ToHashSet();

        var existing = rolePermissionRepository.Table
            .Where(x => x.RoleId == role.Id)
            .ToList();

        foreach (var permissionId in permissionIds)
        {
            if (existing.Any(x => x.PermissionId == permissionId))
                continue;

            rolePermissionRepository.Add(new AccountRolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId,
                CreatedAt = now,
                UpdatedAt = now
            }, saveNow: false);
        }
    }

    private void MapPlanPermissions(string planCode, IReadOnlyCollection<string> permissionKeys, DateTimeOffset now)
    {
        var planId = planRepository.TableNoTracking
            .Where(x => x.Code == planCode)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (planId <= 0)
            return;

        var permissionIds = permissionRepository.TableNoTracking
            .Where(x => permissionKeys.Contains(x.Key))
            .Select(x => x.Id)
            .ToHashSet();

        var existing = planPermissionRepository.Table
            .Where(x => x.PlanId == planId)
            .ToList();

        foreach (var permissionId in permissionIds)
        {
            if (existing.Any(x => x.PermissionId == permissionId))
                continue;

            planPermissionRepository.Add(new AccountPlanPermission
            {
                PlanId = planId,
                PermissionId = permissionId,
                CreatedAt = now,
                UpdatedAt = now
            }, saveNow: false);
        }
    }

    private void EnsureSeedUserPlan(string planCode, DateTimeOffset now)
    {
        var planId = planRepository.TableNoTracking
            .Where(x => x.Code == planCode)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (planId <= 0 || !Guid.TryParse(DataInitializerDefaultValues.UserId, out var seedUserId))
            return;

        var exists = userPlanSubscriptionRepository.TableNoTracking
            .Any(x => x.UserId == seedUserId && x.PlanId == planId && x.IsActive);

        if (exists)
            return;

        userPlanSubscriptionRepository.Add(new AccountUserPlanSubscription
        {
            UserId = seedUserId,
            PlanId = planId,
            IsActive = true,
            StartsAt = now.AddDays(-1),
            EndsAt = null,
            CreatedAt = now,
            UpdatedAt = now
        }, saveNow: false);
    }
}
