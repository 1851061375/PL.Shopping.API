namespace TD.WebApi.Application.Catalog.AppConfigs;

public class DeleteAppConfigRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }

    public DeleteAppConfigRequest(Guid id) => Id = id;
}

public class DeleteAppConfigRequestHandler : IRequestHandler<DeleteAppConfigRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<AppConfig> _repo;
    private readonly IStringLocalizer<DeleteAppConfigRequestHandler> _localizer;

    public DeleteAppConfigRequestHandler(IRepositoryWithEvents<AppConfig> AppConfigRepo,  IStringLocalizer<DeleteAppConfigRequestHandler> localizer) =>
        (_repo,  _localizer) = (AppConfigRepo,  localizer);

    public async Task<Result<Guid>> Handle(DeleteAppConfigRequest request, CancellationToken cancellationToken)
    {


        var item = await _repo.GetByIdAsync(request.Id, cancellationToken);

        _ = item ?? throw new NotFoundException(_localizer["AppConfig.notfound"]);

        await _repo.DeleteAsync(item, cancellationToken);

        return Result<Guid>.Success(request.Id);
    }
}