namespace TD.WebApi.Application.Catalog.CategoryGroups;

public class CreateCategoryGroupRequest : IRequest<Result<Guid>>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsSystem { get; set; } = false;
    public bool? IsActive { get; set; } = true;
}

public class CreateCategoryGroupRequestValidator : CustomValidator<CreateCategoryGroupRequest>
{
    public CreateCategoryGroupRequestValidator(IReadRepository<CategoryGroup> repository, IStringLocalizer<CreateCategoryGroupRequestValidator> T) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync(async (name, ct) => await repository.FirstOrDefaultAsync(new CategoryGroupByCodeSpec(name), ct) is null)
                .WithMessage((_, name) => T["CategoryGroup {0} already Exists.", name]);
}

public class CreateCategoryGroupRequestHandler : IRequestHandler<CreateCategoryGroupRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<CategoryGroup> _repository;

    public CreateCategoryGroupRequestHandler(IRepositoryWithEvents<CategoryGroup> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateCategoryGroupRequest request, CancellationToken cancellationToken)
    {
        var item = new CategoryGroup(request.Name, request.Code, request.Description, request.SortOrder, request.IsActive);

        await _repository.AddAsync(item, cancellationToken);

        return Result<Guid>.Success(item.Id);
    }
}