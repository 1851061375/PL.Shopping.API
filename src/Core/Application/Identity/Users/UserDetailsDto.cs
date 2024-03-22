namespace TD.WebApi.Application.Identity.Users;

public class UserDetailsDto
{
    public string? Id { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public bool? IsActive { get; set; } = true;

    public bool? EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; }
    public string? ImageUrl { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProvinceCode { get; set; }
    public string? ProvinceName { get; set; }
    public string? DistrictCode { get; set; }
    public string? DistrictName { get; set; }
    public string? WardCode { get; set; }
    public string? WardName { get; set; }
    public string? Address { get; set; }
    /// <summary>
    /// Loại tài khoản
    /// 0: Mặc định : người dùng hệ thống
    /// 1: Học sinh
    /// 2: Phụ huynh
    /// 3: Giáo viên
    /// 4: 
    /// </summary>
    public int? Type { get; set; }
    /// <summary>
    /// Là tài khoản đặc biệt - Có thể đăng ký các khóa học ưu tiên không mất phí
    /// </summary>
    public bool? IsSpecial { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? RefCode { get; set; }
    public string? MyRefCode { get; set; }
    public string? Introduction { get; set; }
    public string? DiscountConfig { get; set; }
    public ICollection<string>? Permissions { get; set; }
}