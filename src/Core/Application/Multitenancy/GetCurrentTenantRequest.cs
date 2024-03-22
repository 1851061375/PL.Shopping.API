namespace TD.WebApi.Application.Multitenancy;

public class GetCurrentTenantRequest : IRequest<CurrentTenantDto>
{

}

public class GetCurrentTenantRequestHandler : IRequestHandler<GetCurrentTenantRequest, CurrentTenantDto>
{
    private readonly ITenantService _tenantService;

    public GetCurrentTenantRequestHandler(ITenantService tenantService) => _tenantService = tenantService;

    public Task<CurrentTenantDto> Handle(GetCurrentTenantRequest request, CancellationToken cancellationToken) =>
        _tenantService.GetCurrentAsync();
}