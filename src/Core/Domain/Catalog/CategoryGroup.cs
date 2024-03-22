namespace TD.WebApi.Domain.Catalog;

/// <summary>
/// Nhóm danh mục
/// </summary>
public class CategoryGroup : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsSystem { get; set; } = false;
    public bool? IsActive { get; set; } = true;

    public CategoryGroup(string name, string code, string? description, int? sortOrder, bool? isActive)
    {
        Name = name;
        Code = code;
        Description = description;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public CategoryGroup Update(string name, string code, string? description, int? sortOrder, bool? isActive)
    {
        Name = name;
        Code = code;
        Description = description;
        SortOrder = sortOrder;
        IsActive = isActive;
        return this;
    }
}