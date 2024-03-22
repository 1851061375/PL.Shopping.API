namespace TD.WebApi.Application.Catalog.Categories;

public class CreateCategoryRequest : IRequest<Result<Guid>>
{
    public Guid? CategoryGroupId { get; set; }
    public string Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; } = true;
}

public class CreateCategoryRequestValidator : CustomValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator(IReadRepository<Category> repository, IStringLocalizer<CreateCategoryRequestValidator> T) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync(async (name, ct) => await repository.FirstOrDefaultAsync(new CategoryByCodeSpec(name), ct) is null)
                .WithMessage((_, name) => T["Category {0} already Exists.", name]);
}

public class CreateCategoryRequestHandler : IRequestHandler<CreateCategoryRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<Category> _repository;

    public CreateCategoryRequestHandler(IRepositoryWithEvents<Category> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var item = new Category(request.CategoryGroupId, request.Name, request.Code, request.Description, request.SortOrder, request.IsActive);

        await _repository.AddAsync(item, cancellationToken);

        return Result<Guid>.Success(item.Id);
    }
}