namespace TD.WebApi.Application.Identity.Users;

public class UpdateUserRequestValidator : CustomValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(IUserService userService, IStringLocalizer<UpdateUserRequestValidator> T)
    {
        RuleFor(p => p.Id)
            .NotEmpty();
    }
}