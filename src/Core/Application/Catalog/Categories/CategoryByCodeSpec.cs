namespace TD.WebApi.Application.Catalog.Categories;

public class CategoryByCodeSpec : Specification<Category>, ISingleResultSpecification<Category>
{
    public CategoryByCodeSpec(string code) =>
        Query.Where(b => b.Code == code);
}