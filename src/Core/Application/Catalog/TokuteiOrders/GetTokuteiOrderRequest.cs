namespace TD.WebApi.Application.Catalog.TokuteiOrders;

public class GetTokuteiOrderRequest : IRequest<Result<TokuteiOrderDto>>
{
    public Guid Id { get; set; }

    public GetTokuteiOrderRequest(Guid id) => Id = id;
}

public class TokuteiOrderByIdSpec : Specification<TokuteiOrder, TokuteiOrderDto>, ISingleResultSpecification
{
    public TokuteiOrderByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id);
}

public class GetTokuteiOrderRequestHandler : IRequestHandler<GetTokuteiOrderRequest, Result<TokuteiOrderDto>>
{
    private readonly IRepository<TokuteiOrder> _repository;
    private readonly IStringLocalizer _t;

    public GetTokuteiOrderRequestHandler(IRepository<TokuteiOrder> repository, IStringLocalizer<GetTokuteiOrderRequestHandler> localizer) => (_repository, _t) = (repository, localizer);

    public async Task<Result<TokuteiOrderDto>> Handle(GetTokuteiOrderRequest request, CancellationToken cancellationToken)
    {
        var item = await _repository.FirstOrDefaultAsync(
            (ISpecification<TokuteiOrder, TokuteiOrderDto>)new TokuteiOrderByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(_t["TokuteiOrder {0} Not Found.", request.Id]);
        return Result<TokuteiOrderDto>.Success(item);
    }
}