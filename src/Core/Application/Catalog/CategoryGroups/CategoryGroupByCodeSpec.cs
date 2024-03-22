namespace TD.WebApi.Application.Catalog.CategoryGroups;

public class CategoryGroupByCodeSpec : Specification<CategoryGroup>, ISingleResultSpecification<CategoryGroup>
{
    public CategoryGroupByCodeSpec(string code) =>
        Query.Where(b => b.Code == code);
}