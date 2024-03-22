using Finbuckle.MultiTenant;
using TD.WebApi.Shared.Multitenancy;

namespace TD.WebApi.Infrastructure.Multitenancy;

public class TDTenantInfo : ITenantInfo
{
    public TDTenantInfo()
    {
    }

    public TDTenantInfo(string id, string name, string? connectionString, string adminEmail, string? issuer = null)
    {
        Id = id;
        Identifier = id;
        Name = name;
        ConnectionString = connectionString ?? string.Empty;
        AdminEmail = adminEmail;
        IsActive = true;
        Issuer = issuer;

        // Add Default 1 Month Validity for all new tenants. Something like a DEMO period for tenants.
        ValidUpto = DateTime.Now.AddMonths(1);
    }

    public TDTenantInfo(string id, string name, string? connectionString, string adminEmail, string? issuer, DateTime? dateOfEstablishment, string? tax, string? logo, string? transactionName, string? businessRegistrationCode, DateTime? dateOfIssue, string? placeOfIssue, string? representativeName, string? representativeTitle, string? address, string? provinceCode, string? provinceName, string? districtCode, string? districtName, string? wardCode, string? wardName, string? email, string? phoneNumber, string? fax, string? website)
    {
        Id = id;
        Identifier = id;
        Name = name;
        ConnectionString = connectionString ?? string.Empty;
        AdminEmail = adminEmail;
        IsActive = true;
        Issuer = issuer;

        // Add Default 1 Month Validity for all new tenants. Something like a DEMO period for tenants.
        ValidUpto = DateTime.UtcNow.AddMonths(1);

        DateOfEstablishment = dateOfEstablishment;
        Tax = tax;
        Logo = logo;
        TransactionName = transactionName;
        BusinessRegistrationCode = businessRegistrationCode;
        DateOfIssue = dateOfIssue;
        PlaceOfIssue = placeOfIssue;
        RepresentativeName = representativeName;
        RepresentativeTitle = representativeTitle;
        Address = address;
        ProvinceCode = provinceCode;
        ProvinceName = provinceName;
        DistrictCode = districtCode;
        DistrictName = districtName;
        WardCode = wardCode;
        WardName = wardName;
        Email = email;
        PhoneNumber = phoneNumber;
        Fax = fax;
        Website = website;
    }

    public TDTenantInfo(string id, string name, string? connectionString, string adminEmail, string? issuer, DateTime? dateOfEstablishment, string? tax, string? logo, string? transactionName, string? businessRegistrationCode, DateTime? dateOfIssue, string? placeOfIssue, string? representativeName, string? representativeTitle, string? address, string? provinceCode, string? provinceName, string? districtCode, string? districtName, string? wardCode, string? wardName, string? email, string? phoneNumber, string? fax, string? website, string? investor, string constructor, string? architect, string? supervisor, string? description)
    {
        Id = id;
        Identifier = id;
        Name = name;
        ConnectionString = connectionString ?? string.Empty;
        AdminEmail = adminEmail;
        IsActive = true;
        Issuer = issuer;

        // Add Default 1 Month Validity for all new tenants. Something like a DEMO period for tenants.
        ValidUpto = DateTime.UtcNow.AddMonths(1);

        DateOfEstablishment = dateOfEstablishment;
        Tax = tax;
        Logo = logo;
        TransactionName = transactionName;
        BusinessRegistrationCode = businessRegistrationCode;
        DateOfIssue = dateOfIssue;
        PlaceOfIssue = placeOfIssue;
        RepresentativeName = representativeName;
        RepresentativeTitle = representativeTitle;
        Address = address;
        ProvinceCode = provinceCode;
        ProvinceName = provinceName;
        DistrictCode = districtCode;
        DistrictName = districtName;
        WardCode = wardCode;
        WardName = wardName;
        Email = email;
        PhoneNumber = phoneNumber;
        Fax = fax;
        Website = website;
        Investor = investor;
        Constructor = constructor;
        Architect = architect;
        Supervisor = supervisor;
        Description = description;
    }



    /// <summary>
    /// The actual TenantId, which is also used in the TenantId shadow property on the multitenant entities.
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// The identifier that is used in headers/routes/querystrings. This is set to the same as Id to avoid confusion.
    /// </summary>
    public string Identifier { get; set; } = default!;
    /// <summary>
    /// Tên công ty, dự án bất động sản (với bài toán quản lý tòa nhà)
    /// </summary>
    public string Name { get; set; } = default!;
    public string ConnectionString { get; set; } = default!;
    public string AdminEmail { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public DateTime ValidUpto { get; private set; }

    /// <summary>
    /// Used by AzureAd Authorization to store the AzureAd Tenant Issuer to map against.
    /// </summary>
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

    /// <summary>
    /// Chủ đầu tư
    /// </summary>
    public string? Investor { get; set; }
    /// <summary>
    /// Nhà thầu
    /// </summary>
    public string? Constructor { get; set; }
    /// <summary>
    /// Kiến trúc sư
    /// </summary>
    public string? Architect { get; set; }
    /// <summary>
    /// Giám sát
    /// </summary>
    public string? Supervisor { get; set; }
    public string? Description { get; set; }


    public string? Fax { get; set; }
    public string? Website { get; set; }



    public void AddValidity(int months) =>
        ValidUpto = ValidUpto.AddMonths(months);

    public void SetValidity(in DateTime validTill) =>
        ValidUpto = ValidUpto < validTill
            ? validTill
            : throw new Exception("Subscription cannot be backdated.");

    public void Activate()
    {
        if (Id == MultitenancyConstants.Root.Id)
        {
            throw new InvalidOperationException("Invalid Tenant");
        }

        IsActive = true;
    }

    public void Deactivate()
    {
        if (Id == MultitenancyConstants.Root.Id)
        {
            throw new InvalidOperationException("Invalid Tenant");
        }

        IsActive = false;
    }

    string? ITenantInfo.Id { get => Id; set => Id = value ?? throw new InvalidOperationException("Id can't be null."); }
    string? ITenantInfo.Identifier { get => Identifier; set => Identifier = value ?? throw new InvalidOperationException("Identifier can't be null."); }
    string? ITenantInfo.Name { get => Name; set => Name = value ?? throw new InvalidOperationException("Name can't be null."); }
    string? ITenantInfo.ConnectionString { get => ConnectionString; set => ConnectionString = value ?? throw new InvalidOperationException("ConnectionString can't be null."); }
}