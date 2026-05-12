using Common.Markers;
using Entities.Accounts;
using Entities.Licenses;
using Microsoft.AspNetCore.Identity;
using Services.Contracts.Authentications;
using Services.Contracts.Repositories;
using Services.DTOs.Accounts.Authorization;

namespace Services.Services.Authentications;

public class AuthorizationManagementService(
    RoleManager<Role> roleManager,
    IRepository<AccountPermission> permissionRepository,
    IRepository<AccountRolePermission> rolePermissionRepository,
    IRepository<AccountPlanPermission> planPermissionRepository,
    IRepository<AccountUserPermissionGrant> userGrantRepository,
    IRepository<AccountUserPermissionRevoke> userRevokeRepository,
    IRepository<AccountUserPlanSubscription> userPlanSubscriptionRepository,
    IRepository<LicensePlan> licensePlanRepository,
    IEffectiveAuthorizationService effectiveAuthorizationService) : IAuthorizationManagementService, IScopedDependency
{
    public async Task UpsertRolePermissionsAsync(UpsertRolePermissionsRequest request, Guid actorUserId, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByNameAsync(request.RoleName)
            ?? throw new NotFoundException($"Role '{request.RoleName}' not found.");
        var permissionIds = await ResolvePermissionIdsAsync(request.PermissionKeys, cancellationToken);

        var existing = await rolePermissionRepository.Table
            .Where(x => x.RoleId == role.Id)
            .ToListAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var existingIds = existing.Select(x => x.PermissionId).ToHashSet();
        var incomingIds = permissionIds.ToHashSet();

        foreach (var row in existing.Where(x => !incomingIds.Contains(x.PermissionId)))
        {
            await rolePermissionRepository.DeleteAsync(row, cancellationToken, saveNow: false);
        }

        foreach (var permissionId in incomingIds.Where(id => !existingIds.Contains(id)))
        {
            await rolePermissionRepository.AddAsync(new AccountRolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId,
                UpdatedByUserId = actorUserId,
                CreatedAt = now,
                UpdatedAt = now
            }, cancellationToken, saveNow: false);
        }

        await rolePermissionRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertPlanPermissionsAsync(UpsertPlanPermissionsRequest request, Guid actorUserId, CancellationToken cancellationToken)
    {
        var plan = await licensePlanRepository.Table
            .FirstOrDefaultAsync(x => x.Code == request.PlanCode && x.IsActive, cancellationToken)
            ?? throw new NotFoundException($"Plan '{request.PlanCode}' not found.");

        var permissionIds = await ResolvePermissionIdsAsync(request.PermissionKeys, cancellationToken);
        var existing = await planPermissionRepository.Table
            .Where(x => x.PlanId == plan.Id)
            .ToListAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var existingIds = existing.Select(x => x.PermissionId).ToHashSet();
        var incomingIds = permissionIds.ToHashSet();

        foreach (var row in existing.Where(x => !incomingIds.Contains(x.PermissionId)))
        {
            await planPermissionRepository.DeleteAsync(row, cancellationToken, saveNow: false);
        }

        foreach (var permissionId in incomingIds.Where(id => !existingIds.Contains(id)))
        {
            await planPermissionRepository.AddAsync(new AccountPlanPermission
            {
                PlanId = plan.Id,
                PermissionId = permissionId,
                UpdatedByUserId = actorUserId,
                CreatedAt = now,
                UpdatedAt = now
            }, cancellationToken, saveNow: false);
        }

        await planPermissionRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertUserPermissionGrantAsync(UpsertUserPermissionGrantRequest request, Guid actorUserId, CancellationToken cancellationToken)
    {
        var permissionId = await ResolvePermissionIdAsync(request.PermissionKey, cancellationToken);
        var now = DateTimeOffset.UtcNow;

        var row = await userGrantRepository.Table
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.PermissionId == permissionId, cancellationToken);

        if (row == null)
        {
            await userGrantRepository.AddAsync(new AccountUserPermissionGrant
            {
                UserId = request.UserId,
                PermissionId = permissionId,
                Source = string.IsNullOrWhiteSpace(request.Source) ? "manual" : request.Source.Trim(),
                Notes = request.Notes?.Trim(),
                CreatedAt = now,
                UpdatedAt = now,
                UpdatedByUserId = actorUserId
            }, cancellationToken);
        }
        else
        {
            row.Source = string.IsNullOrWhiteSpace(request.Source) ? row.Source : request.Source.Trim();
            row.Notes = request.Notes?.Trim();
            row.UpdatedAt = now;
            row.UpdatedByUserId = actorUserId;
            await userGrantRepository.UpdateAsync(row, cancellationToken);
        }

        await effectiveAuthorizationService.InvalidateUserAuthorizationAsync(request.UserId, cancellationToken);
    }

    public async Task UpsertUserPermissionRevokeAsync(UpsertUserPermissionRevokeRequest request, Guid actorUserId, CancellationToken cancellationToken)
    {
        var permissionId = await ResolvePermissionIdAsync(request.PermissionKey, cancellationToken);
        var now = DateTimeOffset.UtcNow;
        var row = await userRevokeRepository.Table
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.PermissionId == permissionId, cancellationToken);

        if (row == null)
        {
            await userRevokeRepository.AddAsync(new AccountUserPermissionRevoke
            {
                UserId = request.UserId,
                PermissionId = permissionId,
                Notes = request.Notes?.Trim(),
                CreatedAt = now,
                UpdatedAt = now,
                UpdatedByUserId = actorUserId
            }, cancellationToken);
        }
        else
        {
            row.Notes = request.Notes?.Trim();
            row.UpdatedAt = now;
            row.UpdatedByUserId = actorUserId;
            await userRevokeRepository.UpdateAsync(row, cancellationToken);
        }

        await effectiveAuthorizationService.InvalidateUserAuthorizationAsync(request.UserId, cancellationToken);
    }

    public async Task UpsertUserPlanSubscriptionAsync(UpsertUserPlanSubscriptionRequest request, Guid actorUserId, CancellationToken cancellationToken)
    {
        var plan = await licensePlanRepository.Table
            .FirstOrDefaultAsync(x => x.Code == request.PlanCode && x.IsActive, cancellationToken)
            ?? throw new NotFoundException($"Plan '{request.PlanCode}' not found.");

        var row = await userPlanSubscriptionRepository.Table
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.PlanId == plan.Id, cancellationToken);

        var now = DateTimeOffset.UtcNow;
        if (row == null)
        {
            await userPlanSubscriptionRepository.AddAsync(new AccountUserPlanSubscription
            {
                UserId = request.UserId,
                PlanId = plan.Id,
                StartsAt = request.StartsAt,
                EndsAt = request.EndsAt,
                IsActive = request.IsActive,
                CreatedAt = now,
                UpdatedAt = now,
                UpdatedByUserId = actorUserId
            }, cancellationToken);
        }
        else
        {
            row.StartsAt = request.StartsAt;
            row.EndsAt = request.EndsAt;
            row.IsActive = request.IsActive;
            row.UpdatedAt = now;
            row.UpdatedByUserId = actorUserId;
            await userPlanSubscriptionRepository.UpdateAsync(row, cancellationToken);
        }

        await effectiveAuthorizationService.InvalidateUserAuthorizationAsync(request.UserId, cancellationToken);
    }

    private async Task<List<long>> ResolvePermissionIdsAsync(IReadOnlyList<string> permissionKeys, CancellationToken cancellationToken)
    {
        var normalizedKeys = permissionKeys
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var rows = await permissionRepository.TableNoTracking
            .Where(x => normalizedKeys.Contains(x.Key) && x.IsActive)
            .ToListAsync(cancellationToken);

        if (rows.Count != normalizedKeys.Count)
        {
            var foundKeys = rows.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var missing = normalizedKeys.Where(x => !foundKeys.Contains(x)).ToList();
            throw new BadRequestException($"Unknown permission key(s): {string.Join(", ", missing)}");
        }

        return rows.Select(x => x.Id).ToList();
    }

    private async Task<long> ResolvePermissionIdAsync(string permissionKey, CancellationToken cancellationToken)
    {
        var row = await permissionRepository.TableNoTracking
            .FirstOrDefaultAsync(x => x.Key == permissionKey.Trim() && x.IsActive, cancellationToken)
            ?? throw new BadRequestException($"Unknown permission key '{permissionKey}'.");

        return row.Id;
    }
}
