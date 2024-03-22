namespace TD.WebApi.Application.Identity.Permissions;

public class UpdateUserPermissionsRequest
{
    public string Id { get; set; } = default!;
    public List<string> Permissions { get; set; } = default!;
}

public class UpdateUserPermissionsRequestValidator : CustomValidator<UpdateUserPermissionsRequest>
{
    public UpdateUserPermissionsRequestValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty();
        RuleFor(r => r.Permissions)
            .NotNull();
    }
}