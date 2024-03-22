namespace TD.WebApi.Application.Identity.Users;

public class ChangeTypeRequest
{
    public int Type { get; set; }
    public string? UserId { get; set; }
}
