using Ardalis.Specification.EntityFrameworkCore;
using Finbuckle.MultiTenant;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading;
using TD.WebApi.Application.Common.Events;
using TD.WebApi.Application.Common.Exceptions;
using TD.WebApi.Application.Common.Interfaces;
using TD.WebApi.Application.Common.Models;
using TD.WebApi.Application.Common.Persistence;
using TD.WebApi.Application.Identity.Permissions;
using TD.WebApi.Application.Identity.Roles;
using TD.WebApi.Domain.Identity;
using TD.WebApi.Infrastructure.Persistence.Context;
using TD.WebApi.Shared.Authorization;
using TD.WebApi.Shared.Multitenancy;

namespace TD.WebApi.Infrastructure.Identity;

internal class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly ITenantInfo _currentTenant;
    private readonly IEventPublisher _events;
    private readonly IDapperRepository _repository;
    private readonly IPermissionService _permissionService;

    public RoleService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext db,
        IStringLocalizer<RoleService> localizer,
        ICurrentUser currentUser,
        ITenantInfo currentTenant,
        IDapperRepository repository,
        IPermissionService permissionService,
        IEventPublisher events)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _db = db;
        _t = localizer;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _events = events;
        _repository = repository;
        _permissionService = permissionService;
    }

    public async Task<PaginationResponse<RoleDetailsDto>> SearchAsync(RoleListFilter filter, CancellationToken cancellationToken)
    {
        string query = @"SELECT Roles.*";
        query += @"FROM [Identity].[Roles] AS Roles ";

        string where = " ";
        if (!string.IsNullOrEmpty(filter.Keyword))
        {
            where += $" AND (Roles.Name LIKE '%{filter.Keyword}%') ";
        }

        where = " WHERE Roles.TenantId = '@tenant' " + where;
        string whereOrder = " ORDER BY Main.Name DESC ";

        string paging = $" OFFSET {(filter.PageNumber - 1) * filter.PageSize} ROWS FETCH NEXT {filter.PageSize} ROWS ONLY";

        string sql = $"WITH Main AS ({query} {where}), TotalCount AS (SELECT COUNT(ID) AS [TotalCount]  FROM Main) SELECT * FROM Main, TotalCount {whereOrder} {paging} ";

        return await _repository.PaginatedListNewAsync<RoleDetailsDto>(sql, filter.PageNumber, filter.PageSize, cancellationToken);
    }

    public async Task<List<RoleDto>> GetListAsync(CancellationToken cancellationToken) =>
        (await _roleManager.Roles.ToListAsync(cancellationToken))
            .Adapt<List<RoleDto>>();

    public async Task<int> GetCountAsync(CancellationToken cancellationToken) =>
        await _roleManager.Roles.CountAsync(cancellationToken);

    public async Task<bool> ExistsAsync(string roleName, string? excludeId) =>
        await _roleManager.FindByNameAsync(roleName)
            is ApplicationRole existingRole
            && existingRole.Id != excludeId;

    public async Task<RoleDto> GetByIdAsync(string id) =>
        await _db.Roles.SingleOrDefaultAsync(x => x.Id == id) is { } role
            ? role.Adapt<RoleDto>()
            : throw new NotFoundException(_t["Role Not Found"]);

    public async Task<RoleDto> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken)
    {
        var role = await GetByIdAsync(roleId);

        role.Permissions = await _db.RoleClaims
            .Where(c => c.RoleId == roleId && c.ClaimType == TDClaims.Permission)
            .Select(c => c.ClaimValue!)
            .ToListAsync(cancellationToken);

        return role;
    }

    public async Task<string> CreateOrUpdateAsync(CreateOrUpdateRoleRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            // Create a new role.
            var role = new ApplicationRole(request.Name, request.Description, false);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                throw new InternalServerException(_t["Register role failed"], result.GetErrors(_t));
            }

            await _events.PublishAsync(new ApplicationRoleCreatedEvent(role.Id, role.Name!));


            await UpdatePermissionsAsync(role, request.Permissions, cancellationToken);


            return string.Format(_t["Role {0} Created."], request.Name);
        }
        else
        {
            // Update an existing role.
            var role = await _roleManager.FindByIdAsync(request.Id);

            _ = role ?? throw new NotFoundException(_t["Role Not Found"]);

            if (TDRoles.IsDefault(role.Name!))
            {
                throw new ConflictException(string.Format(_t["Not allowed to modify {0} Role."], role.Name));
            }

            role.Name = request.Name;
            role.NormalizedName = request.Name.ToUpperInvariant();
            role.Description = request.Description;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                throw new InternalServerException(_t["Update role failed"], result.GetErrors(_t));
            }

            await _events.PublishAsync(new ApplicationRoleUpdatedEvent(role.Id, role.Name));

            await UpdatePermissionsAsync(role, request.Permissions, cancellationToken);


            return string.Format(_t["Role {0} Updated."], role.Name);
        }
    }

    public async Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId);
        _ = role ?? throw new NotFoundException(_t["Role Not Found"]);
        if (role.Name == TDRoles.Admin)
        {
            throw new ConflictException(_t["Not allowed to modify Permissions for this Role."]);
        }

        if (_currentTenant.Id != MultitenancyConstants.Root.Id)
        {
            // Remove Root Permissions if the Role is not created for Root Tenant.
            request.Permissions.RemoveAll(u => u.StartsWith("Permissions.Root."));
        }

        var currentClaims = await _roleManager.GetClaimsAsync(role);

        // Remove permissions that were previously selected
        foreach (var claim in currentClaims.Where(c => !request.Permissions.Any(p => p == c.Value)))
        {
            var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
            if (!removeResult.Succeeded)
            {
                throw new InternalServerException(_t["Update permissions failed."], removeResult.GetErrors(_t));
            }
        }

        // Add all permissions that were not previously selected
        foreach (string permission in request.Permissions.Where(c => !currentClaims.Any(p => p.Value == c)))
        {
            if (!string.IsNullOrEmpty(permission))
            {
                _db.RoleClaims.Add(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = TDClaims.Permission,
                    ClaimValue = permission,
                    CreatedBy = _currentUser.GetUserId().ToString()
                });
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        await _events.PublishAsync(new ApplicationRoleUpdatedEvent(role.Id, role.Name!, true));

        return _t["Permissions Updated."];
    }

    public async Task<string> DeleteAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        _ = role ?? throw new NotFoundException(_t["Role Not Found"]);

        if (TDRoles.IsDefault(role.Name!))
        {
            throw new ConflictException(string.Format(_t["Not allowed to delete {0} Role."], role.Name));
        }

        if ((await _userManager.GetUsersInRoleAsync(role.Name!)).Count > 0)
        {
            throw new ConflictException(string.Format(_t["Not allowed to delete {0} Role as it is being used."], role.Name));
        }

        await _roleManager.DeleteAsync(role);

        await _events.PublishAsync(new ApplicationRoleDeletedEvent(role.Id, role.Name!));

        return string.Format(_t["Role {0} Deleted."], role.Name);
    }

    public async Task<RoleWithPermissionGroupsDto> GetByIdWithPermissionGroupsAsync(string roleId, CancellationToken cancellationToken)
    {
        var result = await _db.Roles.SingleOrDefaultAsync(x => x.Id == roleId, cancellationToken) is { } role
            ? role.Adapt<RoleWithPermissionGroupsDto>()
            : throw new NotFoundException(_t["Role Not Found"]);

        var allPermissions = _permissionService.GetAllPermissions();

        var permissionsInRole = await _db.RoleClaims
            .Where(c => c.RoleId == roleId && c.ClaimType == TDClaims.Permission)
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
    private async Task<string> UpdatePermissionsAsync(ApplicationRole role, List<PermissionDto>? permissionDtos, CancellationToken cancellationToken)
    {
        if (permissionDtos==null)
        {
            permissionDtos = new List<PermissionDto>();
        }

        if (_currentTenant.Id != MultitenancyConstants.Root.Id)
        {
            // Remove Root Permissions if the Role is not created for Root Tenant.
            permissionDtos.RemoveAll(u => u.Value.StartsWith("Permissions.Root."));
        }

        var currentClaims = await _roleManager.GetClaimsAsync(role);
        var permissions = permissionDtos.Where(x => x.Active).Select(x => x.Value!).ToList();

        // Remove permissions that were previously selected
        foreach (var claim in currentClaims.Where(c => !permissions.Any(p => p == c.Value)))
        {
            var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
            if (!removeResult.Succeeded)
            {
                throw new InternalServerException(_t["Update permissions failed."], removeResult.GetErrors(_t));
            }
        }

        // Add all permissions that were not previously selected
        foreach (string permission in permissions.Where(p => !currentClaims.Any(c => p == c.Value)))
        {
            if (!string.IsNullOrEmpty(permission))
            {
                _db.RoleClaims.Add(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = TDClaims.Permission,
                    ClaimValue = permission,
                    CreatedBy = _currentUser.GetUserId().ToString()
                });
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        await _events.PublishAsync(new ApplicationRoleUpdatedEvent(role.Id, role.Name!, true));
        return _t["Permissions Updated."];
    }

}