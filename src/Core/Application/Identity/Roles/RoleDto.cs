using TD.WebApi.Application.Identity.Permissions;

namespace TD.WebApi.Application.Identity.Roles;

public class RoleDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<string>? Permissions { get; set; }
}

public class RoleWithPermissionsDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<PermissionDto>? Permissions { get; set; } = new List<PermissionDto>();
    public bool? IsUse { get; set; }
}

public class RoleWithPermissionGroupsDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<PermissionGroupDto>? Groups { get; set; } = new List<PermissionGroupDto>();
    public bool? IsUse { get; set; }
}