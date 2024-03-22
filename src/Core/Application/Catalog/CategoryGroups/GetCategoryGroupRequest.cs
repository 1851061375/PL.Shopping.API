namespace TD.WebApi.Application.Catalog.CategoryGroups;

public class GetCategoryGroupRequest : IRequest<Result<CategoryGroupDto>>
{
    public Guid Id { get; set; }

    public GetCategoryGroupRequest(Guid id) => Id = id;
}

public class CategoryGroupByIdSpec : Specification<CategoryGroup, CategoryGroupDto>, ISingleResultSpecification
{
    public CategoryGroupByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id);
}

public class GetCategoryGroupRequestHandler : IRequestHandler<GetCategoryGroupRequest, Result<CategoryGroupDto>>
{
    private readonly IRepository<CategoryGroup> _repository;
    private readonly IStringLocalizer _t;

    public GetCategoryGroupRequestHandler(IRepository<CategoryGroup> repository, IStringLocalizer<GetCategoryGroupRequestHandler> localizer) => (_repository, _t) = (repository, localizer);

    public async Task<Result<CategoryGroupDto>> Handle(GetCategoryGroupRequest request, CancellationToken cancellationToken)
    {
       var item = await _repository.FirstOrDefaultAsync(
           (ISpecification<CategoryGroup, CategoryGroupDto>) new CategoryGroupByIdSpec(request.Id), cancellationToken)
       ?? throw new NotFoundException(_t["CategoryGroup {0} Not Found.", request.Id]);
       return Result<CategoryGroupDto>.Success(item);
    }
}