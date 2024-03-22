namespace TD.WebApi.Application.Identity.Tokens;

public record TokenTFARequest(string? UserName, string? Email, string Code);

public class TokenTFARequestValidator : CustomValidator<TokenTFARequest>
{
    public TokenTFARequestValidator(IStringLocalizer<TokenTFARequestValidator> T)
    {

        RuleFor(p => p).Must(x => !string.IsNullOrEmpty(x.UserName) || !string.IsNullOrEmpty(x.Email))
               .WithMessage(T["Username or Email is required."]);

        RuleFor(p => p.Email).Cascade(CascadeMode.Stop)
            .NotEmpty().When(x => !string.IsNullOrEmpty(x.Email))
            .EmailAddress()
                .WithMessage(T["Invalid Email Address."]);

        RuleFor(p => p.Code).Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}