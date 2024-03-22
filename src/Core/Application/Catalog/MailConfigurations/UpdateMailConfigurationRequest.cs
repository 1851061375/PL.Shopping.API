namespace TD.WebApi.Application.Catalog.MailConfigurations;

public class UpdateMailConfigurationRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string? Name { get; set; }
    /// <summary>
    /// Chủ đề mail
    /// </summary>
    public string? Subject { get; set; }
    public string? Description { get; set; }
    /// <summary>
    /// Nội dung email
    /// </summary>
    public string? Content { get; set; }
    public bool? IsActive { get; set; } = true;
}

public class UpdateMailConfigurationRequestValidator : CustomValidator<UpdateMailConfigurationRequest>
{
    public UpdateMailConfigurationRequestValidator(IRepository<MailConfiguration> repository, IStringLocalizer<UpdateMailConfigurationRequestValidator> T) =>
        RuleFor(p => p.Key)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (item, name, ct) =>
                    await repository.FirstOrDefaultAsync(new MailConfigurationByKeySpec(name), ct)
                        is not MailConfiguration existingItem || existingItem.Id == item.Id)
                .WithMessage((_, name) => T["MailConfiguration {0} already Exists.", name]);
}

public class UpdateMailConfigurationRequestHandler : IRequestHandler<UpdateMailConfigurationRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<MailConfiguration> _repository;
    private readonly IStringLocalizer _t;

    public UpdateMailConfigurationRequestHandler(IRepositoryWithEvents<MailConfiguration> repository, IStringLocalizer<UpdateMailConfigurationRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<Result<Guid>> Handle(UpdateMailConfigurationRequest request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = item
        ?? throw new NotFoundException(_t["MailConfiguration {0} Not Found.", request.Id]);

        item.Update(request.Key, request.Name, request.Subject, request.Description, request.Content, request.IsActive);

        await _repository.UpdateAsync(item, cancellationToken);

        return Result<Guid>.Success(request.Id);
    }
}