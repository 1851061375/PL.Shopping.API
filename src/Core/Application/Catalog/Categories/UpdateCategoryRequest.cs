namespace TD.WebApi.Application.Catalog.Categories;

public class UpdateCategoryRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public Guid? CategoryGroupId { get; set; }
    public string Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; } = true;

    public class UpdateCategoryRequestValidator : CustomValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator(IRepository<Category> repository, IStringLocalizer<UpdateCategoryRequestValidator> T) =>
            RuleFor(p => p.Code)
                .NotEmpty()
                .MaximumLength(256)
                .MustAsync(async (item, name, ct) =>
                        await repository.FirstOrDefaultAsync(new CategoryByCodeSpec(name), ct)
                            is not Category existingItem || existingItem.Id == item.Id)
                    .WithMessage((_, name) => T["Category {0} already Exists.", name]);
    }

    public class UpdateCategoryRequestHandler : IRequestHandler<UpdateCategoryRequest, Result<Guid>>
    {
        // Add Domain Events automatically by using IRepositoryWithEvents
        private readonly IRepositoryWithEvents<Category> _repository;
        private readonly IStringLocalizer _t;

        public UpdateCategoryRequestHandler(IRepositoryWithEvents<Category> repository, IStringLocalizer<UpdateCategoryRequestHandler> localizer) =>
            (_repository, _t) = (repository, localizer);

        public async Task<Result<Guid>> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var item = await _repository.GetByIdAsync(request.Id, cancellationToken);

            _ = item
            ?? throw new NotFoundException(_t["Category {0} Not Found.", request.Id]);

            item.Update(request.CategoryGroupId, request.Name, request.Code, request.Description, request.SortOrder, request.IsActive);

            await _repository.UpdateAsync(item, cancellationToken);

            return Result<Guid>.Success(request.Id);
        }
    }
}