namespace TD.WebApi.Application.Catalog.Categories;

public class DeleteCategoryRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeleteCategoryRequest(Guid id) => Id = id;
}

public class DeleteCategoryRequestHandler : IRequestHandler<DeleteCategoryRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<Category> _repository;
    private readonly IStringLocalizer _t;

    public DeleteCategoryRequestHandler(IRepositoryWithEvents<Category> repository, IStringLocalizer<DeleteCategoryRequestHandler> localizer) =>
        (_repository,  _t) = (repository,  localizer);

    public async Task<Result<Guid>> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = item ?? throw new NotFoundException(_t["Category {0} Not Found."]);

        await _repository.DeleteAsync(item, cancellationToken);

        return Result<Guid>.Success(request.Id);
    }
}