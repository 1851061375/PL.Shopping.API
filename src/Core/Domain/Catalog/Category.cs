namespace TD.WebApi.Domain.Catalog;

/// <summary>
/// Danh mục
/// </summary>
public class Category : AuditableEntity, IAggregateRoot
{
    public Guid? CategoryGroupId { get; set; }
    public string Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; } = true;
    public virtual CategoryGroup? CategoryGroup { get; set; }

    public Category(DefaultIdType? categoryGroupId, string name, string? code, string? description, int? sortOrder, bool? isActive)
    {
        CategoryGroupId = categoryGroupId;
        Name = name;
        Code = code;
        Description = description;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public Category Update(DefaultIdType? categoryGroupId, string name, string? code, string? description, int? sortOrder, bool? isActive)
    {
        CategoryGroupId = categoryGroupId;
        Name = name;
        Code = code;
        Description = description;
        SortOrder = sortOrder;
        IsActive = isActive;
        return this;
    }
}