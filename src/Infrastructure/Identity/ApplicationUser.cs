using Microsoft.AspNetCore.Identity;

namespace TD.WebApi.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProvinceCode { get; set; }
    public string? DistrictCode { get; set; }
    public string? WardCode { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public bool? IsVerified { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public string? ObjectId { get; set; }
    /// <summary>
    /// Loại tài khoản
    /// 0: Mặc định : người dùng hệ thống
    /// 1: Học sinh
    /// 2: Phụ huynh
    /// 3: Giáo viên
    /// 4: Cộng tác viên
    /// 5: Đại lý
    /// </summary>
    public int? Type { get; set; }
    /// <summary>
    /// Là tài khoản đặc biệt - Có thể đăng ký các khóa học ưu tiên không mất phí
    /// </summary>
    public bool? IsSpecial { get; set; }
    public DateTime? CreatedOn { get; set; } = DateTime.Now;
    /// <summary>
    /// Áp dụng mã giới thiệu của người khác
    /// </summary>
    public string? RefCode { get; set; }
    /// <summary>
    /// Mã giới thiệu của bản thân
    /// </summary>
    public string? MyRefCode { get; set; }
    public bool? IsReceiveEmail { get; set; }
    public bool? IsReceivePhoneNumber { get; set; }
    public string? TaxCode { get; set; }
    public string? AgentName { get; set; }
    //Cấu hình chiết khấu cho đại lý, cộng tác viên
    public string? DiscountConfig { get; set; }
    public Guid? CreatedBy { get; set; }
}