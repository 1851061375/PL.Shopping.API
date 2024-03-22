namespace TD.WebApi.Application.Multitenancy;

public class CreateTenantRequest : IRequest<string>
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ConnectionString { get; set; }
    public string AdminEmail { get; set; } = default!;
    public string? Issuer { get; set; }
    /// <summary>
    /// THONG TIN DOANH NGHIEP - TENANT
    /// </summary>
    // Ngay thanh lap
    public DateTime? DateOfEstablishment { get; set; }

    // Ma so thue
    public string? Tax { get; set; }
    public string? Logo { get; set; }
    //Ten tat - ten giao dich
    public string? TransactionName { get; set; }

    /// <summary>
    /// DANG KY KINH DOANH
    /// </summary>

    // Ma so dang ky kinh doanh
    public string? BusinessRegistrationCode { get; set; }
    // Ngay cap
    public DateTime? DateOfIssue { get; set; }
    //Noi cap
    public string? PlaceOfIssue { get; set; }
    //Nguoi dai dien
    public string? RepresentativeName { get; set; }
    //Chuc danh
    public string? RepresentativeTitle { get; set; }

    /// <summary>
    /// LIEN HE
    /// </summary>
    public string? Address { get; set; }
    public string? ProvinceCode { get; set; }
    public string? ProvinceName { get; set; }
    public string? DistrictCode { get; set; }
    public string? DistrictName { get; set; }
    public string? WardCode { get; set; }
    public string? WardName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? Fax { get; set; }
    public string? Website { get; set; }

    /// <summary>
    /// Ma gioi thieu
    /// </summary>
    public string? ReferralCode { get; set; }
    /// <summary>
    /// Ma giam gia
    /// </summary>
    public string? DiscountCode { get; set; }

}

public class CreateTenantRequestHandler : IRequestHandler<CreateTenantRequest, string>
{
    private readonly ITenantService _tenantService;

    public CreateTenantRequestHandler(ITenantService tenantService) => _tenantService = tenantService;

    public Task<string> Handle(CreateTenantRequest request, CancellationToken cancellationToken) =>
        _tenantService.CreateAsync(request, cancellationToken);
}