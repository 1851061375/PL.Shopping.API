namespace TD.WebApi.Application.Catalog.TokuteiOrders;

public class DeleteTokuteiOrderRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeleteTokuteiOrderRequest(Guid id) => Id = id;
}

public class DeleteTokuteiOrderRequestHandler : IRequestHandler<DeleteTokuteiOrderRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<TokuteiOrder> _repository;
    private readonly IStringLocalizer _t;

    public DeleteTokuteiOrderRequestHandler(IRepositoryWithEvents<TokuteiOrder> repository, IStringLocalizer<DeleteTokuteiOrderRequestHandler> localizer) =>
        (_repository,  _t) = (repository,  localizer);

    public async Task<Result<Guid>> Handle(DeleteTokuteiOrderRequest request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = item ?? throw new NotFoundException(_t["TokuteiOrder {0} Not Found."]);

        await _repository.DeleteAsync(item, cancellationToken);

        return Result<Guid>.Success(request.Id);
    }
}