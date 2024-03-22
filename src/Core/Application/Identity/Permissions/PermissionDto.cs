namespace TD.WebApi.Application.Identity.Permissions;
public class PermissionDto
{
    public string? Value { get; set; }
    public string? Description { get; set; }
    public string? Section { get; set; }
    public bool Active { get; set; } = false;
}
