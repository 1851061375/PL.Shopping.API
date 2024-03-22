namespace TD.WebApi.Application.Identity.Roles;

public class RoleListFilter : PaginationFilter
{
    public bool? IsActive { get; set; }
}