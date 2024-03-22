namespace TD.WebApi.Application.Catalog.TokuteiOrders;

public class SearchTokuteiOrderRequest : PaginationFilter, IRequest<PaginationResponse<TokuteiOrderDto>>
{
    public bool? IsActive { get; set; }
}

public class TokuteiOrderBySearchRequestSpec : EntitiesByPaginationFilterSpec<TokuteiOrder, TokuteiOrderDto>
{
    public TokuteiOrderBySearchRequestSpec(SearchTokuteiOrderRequest request)
        : base(request) =>
        Query.OrderBy(c => c.SortOrder, !request.HasOrderBy())
        .Where(p => p.IsActive == request.IsActive, request.IsActive.HasValue);
}

public class SearchTokuteiOrderRequestHandler : IRequestHandler<SearchTokuteiOrderRequest, PaginationResponse<TokuteiOrderDto>>
{
    private readonly IReadRepository<TokuteiOrder> _repository;

    public SearchTokuteiOrderRequestHandler(IReadRepository<TokuteiOrder> repository) => _repository = repository;

    public async Task<PaginationResponse<TokuteiOrderDto>> Handle(SearchTokuteiOrderRequest request, CancellationToken cancellationToken)
    {
        var spec = new TokuteiOrderBySearchRequestSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}