namespace TD.WebApi.Application.Catalog.AppConfigs;

public class SearchAppConfigsRequest : PaginationFilter, IRequest<PaginationResponse<AppConfigDto>>
{
   public bool? IsActivePortal { get; set; }
}

public class AppConfigsBySearchRequestSpec : EntitiesByPaginationFilterSpec<AppConfig, AppConfigDto>
{
    public AppConfigsBySearchRequestSpec(SearchAppConfigsRequest request)
        : base(request) =>
        Query.OrderBy(c => c.Key, !request.HasOrderBy())
        .Where(p => p.IsActivePortal == request.IsActivePortal, request.IsActivePortal.HasValue)
        ;
}

public class SearchAppConfigsRequestHandler : IRequestHandler<SearchAppConfigsRequest, PaginationResponse<AppConfigDto>>
{
    private readonly IReadRepository<AppConfig> _repository;

    public SearchAppConfigsRequestHandler(IReadRepository<AppConfig> repository) => _repository = repository;

    public async Task<PaginationResponse<AppConfigDto>> Handle(SearchAppConfigsRequest request, CancellationToken cancellationToken)
    {
        var spec = new AppConfigsBySearchRequestSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}