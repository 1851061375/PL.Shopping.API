namespace TD.WebApi.Application.Identity.Users;

public class CreateUserRequest
{
    public string? FirstName { get; set; }
    public string? FullName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProvinceCode { get; set; }
    public string? DistrictCode { get; set; }
    public string? WardCode { get; set; }
    public string? Address { get; set; }
    public string? ImageUrl { get; set; }
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
    public int? Type { get; set; } = 0;
}