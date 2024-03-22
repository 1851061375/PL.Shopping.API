namespace TD.WebApi.Application.Identity.Permissions;

public class RoleWithPermissionsDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<PermissionDto>? Permissions { get; set; } = new List<PermissionDto>();
    public bool? IsUse { get; set; }
}

public class UserPermissionDto
{
    public string Id { get; set; } = default!;
    public List<PermissionGroupDto>? Groups { get; set; } = new List<PermissionGroupDto>();
}