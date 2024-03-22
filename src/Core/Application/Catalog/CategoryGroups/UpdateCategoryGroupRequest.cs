namespace TD.WebApi.Application.Catalog.CategoryGroups;

public class UpdateCategoryGroupRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsSystem { get; set; } = false;
    public bool? IsActive { get; set; } = true;
}

public class UpdateCategoryGroupRequestValidator : CustomValidator<UpdateCategoryGroupRequest>
{
    public UpdateCategoryGroupRequestValidator(IRepository<CategoryGroup> repository, IStringLocalizer<UpdateCategoryGroupRequestValidator> T) =>
        RuleFor(p => p.Code)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync(async (item, name, ct) =>
                    await repository.FirstOrDefaultAsync(new CategoryGroupByCodeSpec(name), ct)
                        is not CategoryGroup existingItem || existingItem.Id == item.Id)
                .WithMessage((_, name) => T["CategoryGroup {0} already Exists.", name]);
}

public class UpdateCategoryGroupRequestHandler : IRequestHandler<UpdateCategoryGroupRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<CategoryGroup> _repository;
    private readonly IStringLocalizer _t;

    public UpdateCategoryGroupRequestHandler(IRepositoryWithEvents<CategoryGroup> repository, IStringLocalizer<UpdateCategoryGroupRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<Result<Guid>> Handle(UpdateCategoryGroupRequest request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = item
        ?? throw new NotFoundException(_t["CategoryGroup {0} Not Found.", request.Id]);

        item.Update(request.Name, request.Code, request.Description, request.SortOrder, request.IsActive);

        await _repository.UpdateAsync(item, cancellationToken);

        return Result<Guid>.Success(request.Id);
    }
}