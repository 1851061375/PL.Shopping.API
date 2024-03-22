namespace TD.WebApi.Application.Catalog.LoginLogs;

public partial class CreateLoginLogRequest : IRequest<Result<Guid>>
{
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public Guid? UserId { get; set; }
    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
    public string? BrowserName { get; set; }
    public string? OperatingSystem { get; set; }
    public string? Type { get; set; }
}


public class CreateLoginLogRequestHandler : IRequestHandler<CreateLoginLogRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<LoginLog> _repository;

    public CreateLoginLogRequestHandler(IRepositoryWithEvents<LoginLog> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateLoginLogRequest request, CancellationToken cancellationToken)
    {
        var item = new LoginLog(request.UserName, request.FullName, request.UserId, request.Ip, request.UserAgent, request.BrowserName, request.OperatingSystem, request.Type);
        await _repository.AddAsync(item, cancellationToken);
        return Result<Guid>.Success(item.Id);
    }
}