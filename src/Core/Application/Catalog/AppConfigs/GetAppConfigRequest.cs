namespace TD.WebApi.Application.Catalog.AppConfigs;

public class GetAppConfigRequest : IRequest<Result<AppConfigDetailsDto>>
{
    public Guid Id { get; set; }

    public GetAppConfigRequest(Guid id) => Id = id;
}

public class AppConfigByIdSpec : Specification<AppConfig, AppConfigDetailsDto>, ISingleResultSpecification
{
    public AppConfigByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id);
}

public class GetAppConfigRequestHandler : IRequestHandler<GetAppConfigRequest, Result<AppConfigDetailsDto>>
{
    private readonly IRepository<AppConfig> _repository;
    private readonly IStringLocalizer<GetAppConfigRequestHandler> _localizer;

    public GetAppConfigRequestHandler(IRepository<AppConfig> repository, IStringLocalizer<GetAppConfigRequestHandler> localizer) => (_repository, _localizer) = (repository, localizer);

    public async Task<Result<AppConfigDetailsDto>> Handle(GetAppConfigRequest request, CancellationToken cancellationToken)
    {
        var item = await _repository.FirstOrDefaultAsync(
            (ISpecification<AppConfig, AppConfigDetailsDto>)new AppConfigByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["AppConfig.notfound"], request.Id));
        return Result<AppConfigDetailsDto>.Success(item);

    }
}