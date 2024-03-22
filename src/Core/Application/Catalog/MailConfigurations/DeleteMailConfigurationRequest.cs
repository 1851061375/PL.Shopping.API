namespace TD.WebApi.Application.Catalog.MailConfigurations;

public class DeleteMailConfigurationRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }

    public DeleteMailConfigurationRequest(Guid id) => Id = id;
}

public class DeleteMailConfigurationRequestHandler : IRequestHandler<DeleteMailConfigurationRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<MailConfiguration> _repository;
    private readonly IStringLocalizer _t;

    public DeleteMailConfigurationRequestHandler(IRepositoryWithEvents<MailConfiguration> repository, IStringLocalizer<DeleteMailConfigurationRequestHandler> localizer) =>
        (_repository,  _t) = (repository,  localizer);

    public async Task<Result<Guid>> Handle(DeleteMailConfigurationRequest request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = item ?? throw new NotFoundException(_t["MailConfiguration {0} Not Found."]);

        await _repository.DeleteAsync(item, cancellationToken);

        return Result<Guid>.Success(request.Id);
    }
}