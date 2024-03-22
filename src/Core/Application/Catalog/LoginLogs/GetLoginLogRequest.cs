namespace TD.WebApi.Application.Catalog.LoginLogs;

public class GetLoginLogRequest : IRequest<Result<LoginLogDetailsDto>>
{
    public Guid Id { get; set; }

    public GetLoginLogRequest(Guid id) => Id = id;
}

public class LoginLogByIdSpec : Specification<LoginLog, LoginLogDetailsDto>, ISingleResultSpecification
{
    public LoginLogByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id);
}

public class GetLoginLogRequestHandler : IRequestHandler<GetLoginLogRequest, Result<LoginLogDetailsDto>>
{
    private readonly IRepository<LoginLog> _repository;
    private readonly IStringLocalizer<GetLoginLogRequestHandler> _localizer;

    public GetLoginLogRequestHandler(IRepository<LoginLog> repository, IStringLocalizer<GetLoginLogRequestHandler> localizer) => (_repository, _localizer) = (repository, localizer);

    public async Task<Result<LoginLogDetailsDto>> Handle(GetLoginLogRequest request, CancellationToken cancellationToken)
    {
        var item = await _repository.FirstOrDefaultAsync(
            (ISpecification<LoginLog, LoginLogDetailsDto>)new LoginLogByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["LoginLog.notfound"], request.Id));
        return Result<LoginLogDetailsDto>.Success(item);

    }
}