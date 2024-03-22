namespace TD.WebApi.Application.Identity.Users;

public class CreateUserPublicRequestValidator : CustomValidator<CreateUserPublicRequest>
{
    public CreateUserPublicRequestValidator(IUserService userService, IStringLocalizer<CreateUserPublicRequestValidator> T)
    {
        RuleFor(u => u.UserName).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(6)
            .MustAsync(async (name, _) => !await userService.ExistsWithNameAsync(name))
                .WithMessage((_, name) => T["Tài khoản {0} đã tồn tại trong hệ thống.", name]);

        RuleFor(p => p.Password).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(p => p.ConfirmPassword).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Equal(p => p.Password);
    }
}