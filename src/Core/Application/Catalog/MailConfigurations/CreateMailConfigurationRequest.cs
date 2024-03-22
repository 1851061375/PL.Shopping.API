namespace TD.WebApi.Application.Catalog.MailConfigurations;

public class CreateMailConfigurationRequest : IRequest<Result<Guid>>
{
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

public class CreateMailConfigurationRequestValidator : CustomValidator<CreateMailConfigurationRequest>
{
    public CreateMailConfigurationRequestValidator(IReadRepository<MailConfiguration> repository, IStringLocalizer<CreateMailConfigurationRequestValidator> T) =>
        RuleFor(p => p.Key)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) => await repository.FirstOrDefaultAsync(new MailConfigurationByKeySpec(name), ct) is null)
                .WithMessage((_, name) => T["MailConfiguration {0} already Exists.", name]);
}

public class CreateMailConfigurationRequestHandler : IRequestHandler<CreateMailConfigurationRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<MailConfiguration> _repository;

    public CreateMailConfigurationRequestHandler(IRepositoryWithEvents<MailConfiguration> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateMailConfigurationRequest request, CancellationToken cancellationToken)
    {
        var item = new MailConfiguration(request.Key, request.Name, request.Subject, request.Description, request.Content, request.IsActive);

        await _repository.AddAsync(item, cancellationToken);

        return Result<Guid>.Success(item.Id);
    }
}