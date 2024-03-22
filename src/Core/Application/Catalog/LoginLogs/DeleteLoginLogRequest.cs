namespace TD.WebApi.Application.Catalog.LoginLogs;

public class DeleteLoginLogRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }

    public DeleteLoginLogRequest(Guid id) => Id = id;
}

public class DeleteLoginLogRequestHandler : IRequestHandler<DeleteLoginLogRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<LoginLog> _repo;
    private readonly IStringLocalizer<DeleteLoginLogRequestHandler> _localizer;

    public DeleteLoginLogRequestHandler(IRepositoryWithEvents<LoginLog> LoginLogRepo,  IStringLocalizer<DeleteLoginLogRequestHandler> localizer) =>
        (_repo,  _localizer) = (LoginLogRepo,  localizer);

    public async Task<Result<Guid>> Handle(DeleteLoginLogRequest request, CancellationToken cancellationToken)
    {


        var item = await _repo.GetByIdAsync(request.Id, cancellationToken);

        _ = item ?? throw new NotFoundException(_localizer["LoginLog.notfound"]);

        await _repo.DeleteAsync(item, cancellationToken);

        return Result<Guid>.Success(request.Id);
    }
}