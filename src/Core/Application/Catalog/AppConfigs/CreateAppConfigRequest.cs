namespace TD.WebApi.Application.Catalog.AppConfigs;

public partial class CreateAppConfigRequest : IRequest<Result<Guid>>
{
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? Description { get; set; }
    public bool? IsActivePortal { get; set; }
}

public class CreateAppConfigRequestValidator : CustomValidator<CreateAppConfigRequest>
{
    public CreateAppConfigRequestValidator(IReadRepository<AppConfig> repository, IStringLocalizer<CreateAppConfigRequestValidator> localizer) =>
         RuleFor(p => p.Key)
            .NotEmpty()
            .MaximumLength(512)
            .MustAsync(async (key, ct) => await repository.FirstOrDefaultAsync(new AppConfigByNameSpec(key), ct) is null)
                .WithMessage((_, key) => "Tên cấu hình đã tồn tại");
}

public class CreateAppConfigRequestHandler : IRequestHandler<CreateAppConfigRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<AppConfig> _repository;

    public CreateAppConfigRequestHandler(IRepositoryWithEvents<AppConfig> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateAppConfigRequest request, CancellationToken cancellationToken)
    {
        var item = new AppConfig(request.Key, request.Value, request.Description, request.IsActivePortal);
        await _repository.AddAsync(item, cancellationToken);
        return Result<Guid>.Success(item.Id);
    }
}