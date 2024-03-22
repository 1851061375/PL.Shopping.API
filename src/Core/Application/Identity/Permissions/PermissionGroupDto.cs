namespace TD.WebApi.Application.Identity.Permissions;
public class PermissionGroupDto
{
    public string Section { get; set; } = default!;
    public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
}
