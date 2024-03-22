using Finbuckle.MultiTenant;
using TD.WebApi.Application.Common.Exceptions;
using TD.WebApi.Application.Common.Persistence;
using TD.WebApi.Application.Multitenancy;
using TD.WebApi.Infrastructure.Persistence;
using TD.WebApi.Infrastructure.Persistence.Initialization;
using Mapster;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace TD.WebApi.Infrastructure.Multitenancy;

internal class TenantService : ITenantService
{
    private readonly IMultiTenantStore<TDTenantInfo> _tenantStore;
    private readonly IConnectionStringSecurer _csSecurer;
    private readonly IDatabaseInitializer _dbInitializer;
    private readonly IStringLocalizer _t;
    private readonly DatabaseSettings _dbSettings;
    private readonly TDTenantInfo? _currentTenant;
    public TenantService(
        IMultiTenantStore<TDTenantInfo> tenantStore,
        IConnectionStringSecurer csSecurer,
        IDatabaseInitializer dbInitializer,
        IStringLocalizer<TenantService> localizer,
        TDTenantInfo? currentTenant,
        IOptions<DatabaseSettings> dbSettings)
    {
        _tenantStore = tenantStore;
        _csSecurer = csSecurer;
        _dbInitializer = dbInitializer;
        _t = localizer;
        _dbSettings = dbSettings.Value;
        _currentTenant = currentTenant;
    }

    public async Task<List<TenantDto>> GetAllAsync()
    {
        var tenants = (await _tenantStore.GetAllAsync()).Adapt<List<TenantDto>>();
        tenants.ForEach(t => t.ConnectionString = _csSecurer.MakeSecure(t.ConnectionString));
        return tenants;
    }

    public async Task<bool> ExistsWithIdAsync(string id) =>
        await _tenantStore.TryGetAsync(id) is not null;

    public async Task<bool> ExistsWithNameAsync(string name) =>
        (await _tenantStore.GetAllAsync()).Any(t => t.Name == name);

    public async Task<TenantDto> GetByIdAsync(string id) =>
        (await GetTenantInfoAsync(id))
            .Adapt<TenantDto>();

    public async Task<string> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        if (request.ConnectionString?.Trim() == _dbSettings.ConnectionString.Trim()) request.ConnectionString = string.Empty;

        var tenant = new TDTenantInfo(request.Id, request.Name, request.ConnectionString, request.AdminEmail, request.Issuer, request.DateOfEstablishment, request.Tax, request.Logo, request.TransactionName, request.BusinessRegistrationCode, request.DateOfIssue, request.PlaceOfIssue, request.RepresentativeName, request.RepresentativeTitle, request.Address, request.ProvinceCode, request.ProvinceName, request.DistrictCode, request.DistrictName, request.WardCode, request.WardName, request.Email, request.PhoneNumber, request.Fax, request.Website);
        await _tenantStore.TryAddAsync(tenant);

        // TODO: run this in a hangfire job? will then have to send mail when it's ready or not
        try
        {
            await _dbInitializer.InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
        }
        catch
        {
            await _tenantStore.TryRemoveAsync(request.Id);
            throw;
        }

        return tenant.Id;
    }

    public async Task<string> UpdateAsync(string id, UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = await GetTenantInfoAsync(id);

        tenant.Issuer = request.Issuer;
        tenant.DateOfEstablishment = request.DateOfEstablishment;
        tenant.Website = request.Website;
        tenant.Tax = request.Tax;
        tenant.Logo = request.Logo;
        tenant.TransactionName = request.TransactionName;
        tenant.BusinessRegistrationCode = request.BusinessRegistrationCode;
        tenant.DateOfIssue = request.DateOfIssue;
        tenant.PlaceOfIssue = request.PlaceOfIssue;
        tenant.RepresentativeName = request.RepresentativeName;
        tenant.RepresentativeTitle = request.RepresentativeTitle;
        tenant.Address = request.Address;
        tenant.ProvinceCode = request.ProvinceCode;
        tenant.ProvinceName = request.ProvinceName;
        tenant.DistrictCode = request.DistrictCode;
        tenant.DistrictName = request.DistrictName;
        tenant.WardCode = request.WardCode;
        tenant.WardName = request.WardName;
        tenant.Email = request.Email;
        tenant.PhoneNumber = request.PhoneNumber;
        tenant.Fax = request.Fax;

        // TODO: run this in a hangfire job? will then have to send mail when it's ready or not
        try
        {
            await _tenantStore.TryUpdateAsync(tenant);
        }
        catch
        {
            throw;
        }

        return tenant.Id;
    }

    public async Task<string> ActivateAsync(string id)
    {
        var tenant = await GetTenantInfoAsync(id);

        if (tenant.IsActive)
        {
            throw new ConflictException(_t["Tenant is already Activated."]);
        }

        tenant.Activate();

        await _tenantStore.TryUpdateAsync(tenant);

        return _t["Tenant {0} is now Activated.", id];
    }

    public async Task<string> DeactivateAsync(string id)
    {
        var tenant = await GetTenantInfoAsync(id);
        if (!tenant.IsActive)
        {
            throw new ConflictException(_t["Tenant is already Deactivated."]);
        }

        tenant.Deactivate();
        await _tenantStore.TryUpdateAsync(tenant);
        return _t["Tenant {0} is now Deactivated.", id];
    }

    public async Task<string> UpdateSubscription(string id, DateTime extendedExpiryDate)
    {
        var tenant = await GetTenantInfoAsync(id);
        tenant.SetValidity(extendedExpiryDate);
        await _tenantStore.TryUpdateAsync(tenant);
        return _t["Tenant {0}'s Subscription Upgraded. Now Valid till {1}.", id, tenant.ValidUpto];
    }

    private async Task<TDTenantInfo> GetTenantInfoAsync(string id) =>
        await _tenantStore.TryGetAsync(id)
            ?? throw new NotFoundException(_t["{0} {1} Not Found.", typeof(TDTenantInfo).Name, id]);

    public async Task<CurrentTenantDto> GetCurrentAsync()
    {
        var currentTenant = await _tenantStore.TryGetAsync(_currentTenant?.Id!)
            ?? throw new NotFoundException(_t["{0} {1} Not Found.", typeof(TDTenantInfo).Name, _currentTenant?.Id!]);
        return currentTenant.Adapt<CurrentTenantDto>();
    }
    public async Task<string> UpdateCurrentAsync(UpdateCurrentTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = await GetTenantInfoAsync(_currentTenant?.Id!);

        tenant.Name = request.Name;
        tenant.DateOfEstablishment = request.DateOfEstablishment;
        tenant.Website = request.Website;
        tenant.Tax = request.Tax;
        tenant.Logo = request.Logo;
        tenant.TransactionName = request.TransactionName;
        tenant.BusinessRegistrationCode = request.BusinessRegistrationCode;
        tenant.DateOfIssue = request.DateOfIssue;
        tenant.PlaceOfIssue = request.PlaceOfIssue;
        tenant.RepresentativeName = request.RepresentativeName;
        tenant.RepresentativeTitle = request.RepresentativeTitle;
        tenant.Address = request.Address;
        tenant.ProvinceCode = request.ProvinceCode;
        tenant.ProvinceName = request.ProvinceName;
        tenant.DistrictCode = request.DistrictCode;
        tenant.DistrictName = request.DistrictName;
        tenant.WardCode = request.WardCode;
        tenant.WardName = request.WardName;
        tenant.Email = request.Email;
        tenant.PhoneNumber = request.PhoneNumber;
        tenant.Fax = request.Fax;

        // TODO: run this in a hangfire job? will then have to send mail when it's ready or not
        try
        {
            await _tenantStore.TryUpdateAsync(tenant);
        }
        catch
        {
            throw;
        }

        return tenant.Id;
    }
}