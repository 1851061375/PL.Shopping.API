namespace TD.WebApi.Application.Identity.Tokens;

public record ExternalAuthRequest(string Provider, string ProviderKey, string? UserName, string? Email, string? FullName, string? AvatarUrl, string? PhoneNumber);

public class ExternalAuthRequestValidator : CustomValidator<ExternalAuthRequest>
{
    public ExternalAuthRequestValidator(IStringLocalizer<ExternalAuthRequestValidator> T)
    {
        RuleFor(p => p.Provider).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Invalid Provider.");
        RuleFor(p => p.ProviderKey).NotEmpty().WithMessage("Invalid ProviderKey.");
    }
}