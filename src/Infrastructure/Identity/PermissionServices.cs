using TD.WebApi.Application.Identity.Permissions;
using TD.WebApi.Shared.Authorization;

namespace TD.WebApi.Infrastructure.Identity;
public class PermissionService : IPermissionService
{
    public List<PermissionGroupDto> GetAllPermissionGroups()
    {
        var allPermissions = new List<TDPermission>(TDPermissions.All);
        return allPermissions
            .ConvertAll(x => new PermissionDto
            {
                Description = x.Description,
                Value = TDPermission.NameFor(x.Action, x.Resource),
                Section = x.Section
            })
            .GroupBy(x => x.Section)
            .Select(g => new PermissionGroupDto
            {
                Section = g.Key!,
                Permissions = g.ToList()
            })
            .ToList();
    }

    public List<PermissionDto> GetAllPermissions()
    {
        var allPermissions = new List<TDPermission>(TDPermissions.All);
        return allPermissions.ConvertAll(x => new PermissionDto
        {
            Description = x.Description,
            Value = TDPermission.NameFor(x.Action, x.Resource),
            Section = x.Section
        });
    }
}
