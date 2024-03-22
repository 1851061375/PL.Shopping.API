namespace TD.WebApi.Application.Identity.Users.Password;

public class AdminResetPasswordRequest
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
}