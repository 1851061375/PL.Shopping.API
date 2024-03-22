namespace TD.WebApi.Application.Catalog.Categories;

public class CategoryDto : IDto
{
    public Guid Id { get; set; }
    public Guid? CategoryGroupId { get; set; }
    public string? CategoryGroupCode { get; set; }
    public string? CategoryGroupName { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; } = true;
    public int? TotalCount { get; set; }
}