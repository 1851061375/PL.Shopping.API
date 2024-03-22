namespace TD.WebApi.Application.Catalog.Categories;

public class GetCategoryRequest : IRequest<Result<CategoryDto>>
{
    public Guid Id { get; set; }

    public GetCategoryRequest(Guid id) => Id = id;
}

public class CategoryByIdSpec : Specification<Category, CategoryDto>, ISingleResultSpecification
{
    public CategoryByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id);
}

public class GetCategoryRequestHandler : IRequestHandler<GetCategoryRequest, Result<CategoryDto>>
{
    private readonly IDapperRepository _repository;

    private readonly IStringLocalizer _t;

    public GetCategoryRequestHandler(IDapperRepository repository, IStringLocalizer<GetCategoryRequestHandler> localizer) => (_repository, _t) = (repository, localizer);

    public async Task<Result<CategoryDto>> Handle(GetCategoryRequest request, CancellationToken cancellationToken)
    {
        string sql = @"SELECT Categories.*, CategoryGroups.Name AS CategoryGroupName, CategoryGroups.Code AS CategoryGroupCode  
                    FROM [Catalog].[Categories] Categories
                   LEFT JOIN [Catalog].[CategoryGroups] CategoryGroups ON CategoryGroups.Id = Categories.CategoryGroupId  ";

        var itemResult = await _repository.QueryFirstOrDefaultObjectAsync<CategoryDto>(
            $"{sql} WHERE Categories.Id = '{request.Id}' AND Categories.DeletedOn IS NULL  AND Categories.TenantId = '@tenant'", cancellationToken: cancellationToken);

        _ = itemResult ?? throw new NotFoundException(_t["Categories {0} Not Found.", request.Id]);

        return Result<CategoryDto>.Success(itemResult);
    }
}