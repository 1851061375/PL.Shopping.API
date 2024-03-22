namespace TD.WebApi.Application.Multitenancy;

public class CurrentTenantDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ConnectionString { get; set; }
    public string AdminEmail { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime ValidUpto { get; set; }
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
    public string? ProvinceId { get; set; }
    public string? ProvinceName { get; set; }
    public string? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public string? WardId { get; set; }
    public string? WardName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? Fax { get; set; }
    public string? Website { get; set; }
}