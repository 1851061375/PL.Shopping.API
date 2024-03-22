namespace TD.WebApi.Application.Catalog.MailConfigurations;

public class SearchMailConfigurationsRequest : PaginationFilter, IRequest<PaginationResponse<MailConfigurationDto>>
{
    public bool? IsActive { get; set; }
}

public class MailConfigurationsBySearchRequestSpec : EntitiesByPaginationFilterSpec<MailConfiguration, MailConfigurationDto>
{
    public MailConfigurationsBySearchRequestSpec(SearchMailConfigurationsRequest request)
        : base(request) =>
        Query.OrderBy(c => c.CreatedOn, !request.HasOrderBy())
        .Where(p => p.IsActive == request.IsActive, request.IsActive.HasValue)
        ;
}

public class SearchMailConfigurationsRequestHandler : IRequestHandler<SearchMailConfigurationsRequest, PaginationResponse<MailConfigurationDto>>
{
    private readonly IReadRepository<MailConfiguration> _repository;

    public SearchMailConfigurationsRequestHandler(IReadRepository<MailConfiguration> repository) => _repository = repository;

    public async Task<PaginationResponse<MailConfigurationDto>> Handle(SearchMailConfigurationsRequest request, CancellationToken cancellationToken)
    {
        var spec = new MailConfigurationsBySearchRequestSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}