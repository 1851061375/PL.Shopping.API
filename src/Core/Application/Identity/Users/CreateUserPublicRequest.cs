namespace TD.WebApi.Application.Identity.Users;

public class CreateUserPublicRequest
{
    public string? FirstName { get; set; }
    public string? FullName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public int? Type { get; set; } = 0;
    public string? Introduction { get; set; }
    public string? TaxCode { get; set; }
    public string? AgentName { get; set; }
}