namespace TD.WebApi.Application.Catalog.CategoryGroups;

public class DeleteCategoryGroupRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }

    public DeleteCategoryGroupRequest(Guid id) => Id = id;
}

public class DeleteCategoryGroupRequestHandler : IRequestHandler<DeleteCategoryGroupRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<CategoryGroup> _repository;
    private readonly IStringLocalizer _t;

    public DeleteCategoryGroupRequestHandler(IRepositoryWithEvents<CategoryGroup> repository, IStringLocalizer<DeleteCategoryGroupRequestHandler> localizer) =>
        (_repository,  _t) = (repository,  localizer);

    public async Task<Result<Guid>> Handle(DeleteCategoryGroupRequest request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = item ?? throw new NotFoundException(_t["CategoryGroup {0} Not Found."]);

        await _repository.DeleteAsync(item, cancellationToken);

        return Result<Guid>.Success(request.Id);
    }
}