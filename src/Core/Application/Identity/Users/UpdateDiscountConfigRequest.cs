namespace TD.WebApi.Application.Identity.Users;

public class UpdateDiscountConfigRequest
{
    public string Id { get; set; } = default!;
    public string? DiscountConfig { get; set; }
}