namespace TD.WebApi.Application.Identity.Roles;

public class RoleDetailsDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int? TotalCount { get; set; }
}