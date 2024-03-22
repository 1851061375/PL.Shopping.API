namespace TD.WebApi.Application.Catalog.CategoryGroups;

public class SearchCategoryGroupsRequest : PaginationFilter, IRequest<PaginationResponse<CategoryGroupDto>>
{
    public bool? IsActive { get; set; }
    public bool? IsSystem { get; set; }
}

public class CategoryGroupsBySearchRequestSpec : EntitiesByPaginationFilterSpec<CategoryGroup, CategoryGroupDto>
{
    public CategoryGroupsBySearchRequestSpec(SearchCategoryGroupsRequest request)
        : base(request) =>
        Query.OrderBy(c => c.SortOrder, !request.HasOrderBy())
        .Where(p => p.IsActive == request.IsActive, request.IsActive.HasValue)
        .Where(p => p.IsSystem == request.IsSystem, request.IsSystem.HasValue)
        ;
}

public class SearchCategoryGroupsRequestHandler : IRequestHandler<SearchCategoryGroupsRequest, PaginationResponse<CategoryGroupDto>>
{
    private readonly IReadRepository<CategoryGroup> _repository;

    public SearchCategoryGroupsRequestHandler(IReadRepository<CategoryGroup> repository) => _repository = repository;

    public async Task<PaginationResponse<CategoryGroupDto>> Handle(SearchCategoryGroupsRequest request, CancellationToken cancellationToken)
    {
        var spec = new CategoryGroupsBySearchRequestSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}