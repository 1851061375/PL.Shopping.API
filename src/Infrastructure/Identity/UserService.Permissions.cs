using TD.WebApi.Application.Common.Caching;
using TD.WebApi.Application.Common.Exceptions;
using TD.WebApi.Shared.Authorization;
using Microsoft.EntityFrameworkCore;
using Mapster;
using TD.WebApi.Application.Identity.Permissions;
using TD.WebApi.Application.Identity.Roles;
using TD.WebApi.Domain.Identity;
using TD.WebApi.Shared.Multitenancy;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;

namespace TD.WebApi.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<List<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new UnauthorizedException("Authentication Failed.");

        var userRoles = await _userManager.GetRolesAsync(user);
        var permissions = new List<string>();
        foreach (var role in await _roleManager.Roles
            .Where(r => userRoles.Contains(r.Name!))
            .ToListAsync(cancellationToken))
        {
            permissions.AddRange(await _db.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == TDClaims.Permission)
                .Select(rc => rc.ClaimValue!)
                .ToListAsync(cancellationToken));
        }

        permissions.AddRange(await _db.UserClaims
              .Where(rc => rc.UserId == user.Id && rc.ClaimType == TDClaims.Permission)
              .Select(rc => rc.ClaimValue)
              .ToListAsync(cancellationToken));

        return permissions.Distinct().ToList();
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken)
    {
        var permissions = await _cache.GetOrSetAsync(
            _cacheKeys.GetCacheKey(TDClaims.Permission, userId),
            () => GetPermissionsAsync(userId, cancellationToken),
            cancellationToken: cancellationToken);

        return permissions?.Contains(permission) ?? false;
    }

    public Task InvalidatePermissionCacheAsync(string userId, CancellationToken cancellationToken) =>
        _cache.RemoveAsync(_cacheKeys.GetCacheKey(TDClaims.Permission, userId), cancellationToken);


    public async Task<UserPermissionDto> GetByUserNameWithPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        _ = user ?? throw new NotFoundException("User Not Found.");


        var result = new UserPermissionDto();
        result.Id = user.Id;

        var allPermissions = _permissionService.GetAllPermissions();

        var permissionsInRole = await _db.UserClaims
            .Where(c => c.UserId == userId && c.ClaimType == TDClaims.Permission)
            .Select(c => c.ClaimValue!)
            .ToListAsync(cancellationToken);

        // filter permissions
        foreach (var item in allPermissions)
        {
            if (permissionsInRole.Contains(item.Value!))
            {
                item.Active = true;
            }
        }

        // group permisisons
        var groups = allPermissions.GroupBy(x => x.Section)
            .Select(g => new PermissionGroupDto
            {
                Section = g.Key!,
                Permissions = g.ToList()
            })
            .ToList();

        result.Groups = groups;

        return result;
    }

    public async Task<string> UpdatePermissionsAsync(UpdateUserPermissionsRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        _ = user ?? throw new NotFoundException("User Not Found.");

        if (request.Permissions == null)
        {
            return "Permissions Updated.";
        }

        var currentClaims = await _userManager.GetClaimsAsync(user);

        foreach (var claim in currentClaims.Where(c => !request.Permissions.Any(p => p == c.Value)))
        {
            var removeResult = await _userManager.RemoveClaimAsync(user, claim);
            if (!removeResult.Succeeded)
            {
                throw new InternalServerException("Update permissions failed.");
            }
        }

        foreach (string permission in request.Permissions.Where(c => !currentClaims.Any(p => p.Value == c)))
        {
            if (!string.IsNullOrEmpty(permission))
            {
                _db.UserClaims.Add(new IdentityUserClaim<string>
                {
                    UserId = user.Id,
                    ClaimType = TDClaims.Permission,
                    ClaimValue = permission,
                });
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        return _t["Permissions Updated."];
    }
}