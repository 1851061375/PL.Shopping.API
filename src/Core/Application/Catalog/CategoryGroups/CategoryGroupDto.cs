namespace TD.WebApi.Application.Catalog.CategoryGroups;

public class CategoryGroupDto : IDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsSystem { get; set; } = false;
    public bool? IsActive { get; set; } = true;
    public DateTime? CreatedOn { get; set; }
}