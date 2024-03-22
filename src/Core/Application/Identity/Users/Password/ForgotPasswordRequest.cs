namespace TD.WebApi.Application.Identity.Users.Password;

public class ForgotPasswordRequest
{
    //public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
}

public class ForgotPasswordRequestValidator : CustomValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator(IStringLocalizer<ForgotPasswordRequestValidator> T) =>
        RuleFor(p => p.UserName).Cascade(CascadeMode.Stop)
            .NotEmpty()
            //.EmailAddress()
                .WithMessage(T["Invalid UserName."]);
}