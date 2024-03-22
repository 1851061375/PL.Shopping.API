namespace TD.WebApi.Application.Catalog.Users;

public class UserDto : IDto
{
    public string? Id { get; set; }

    public string? UserName { get; set; }
    public string? FullName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public bool EmailConfirmed { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? PhoneNumber { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public int? TotalCount { get; set; } = 0;
}