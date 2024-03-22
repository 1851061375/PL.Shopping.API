namespace TD.WebApi.Application.Catalog.MailConfigurations;

public class GetMailConfigurationRequest : IRequest<Result<MailConfigurationDetailsDto>>
{
    public Guid Id { get; set; }

    public GetMailConfigurationRequest(Guid id) => Id = id;
}

public class MailConfigurationByIdSpec : Specification<MailConfiguration, MailConfigurationDetailsDto>, ISingleResultSpecification
{
    public MailConfigurationByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id);
}

public class GetMailConfigurationRequestHandler : IRequestHandler<GetMailConfigurationRequest, Result<MailConfigurationDetailsDto>>
{
    private readonly IRepository<MailConfiguration> _repository;
    private readonly IStringLocalizer _t;

    public GetMailConfigurationRequestHandler(IRepository<MailConfiguration> repository, IStringLocalizer<GetMailConfigurationRequestHandler> localizer) => (_repository, _t) = (repository, localizer);

    public async Task<Result<MailConfigurationDetailsDto>> Handle(GetMailConfigurationRequest request, CancellationToken cancellationToken)
    {
       var item = await _repository.FirstOrDefaultAsync(
           (ISpecification<MailConfiguration, MailConfigurationDetailsDto>) new MailConfigurationByIdSpec(request.Id), cancellationToken)
       ?? throw new NotFoundException(_t["MailConfiguration {0} Not Found.", request.Id]);
       return Result<MailConfigurationDetailsDto>.Success(item);
    }
}