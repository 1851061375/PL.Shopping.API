namespace TD.WebApi.Application.Catalog.Notifications;
public class DeleteNotificationRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }

    public DeleteNotificationRequest(Guid id) => Id = id;
}

public class DeleteNotificationRequestHandler : IRequestHandler<DeleteNotificationRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<Notification> _repo;
    private readonly IStringLocalizer<DeleteNotificationRequestHandler> _localizer;

    public DeleteNotificationRequestHandler(IRepositoryWithEvents<Notification> repo, IStringLocalizer<DeleteNotificationRequestHandler> localizer) =>
        (_repo, _localizer) = (repo, localizer);

    public async Task<Result<Guid>> Handle(DeleteNotificationRequest request, CancellationToken cancellationToken)
    {
        var item = await _repo.GetByIdAsync(request.Id, cancellationToken);

        _ = item ?? throw new NotFoundException(_localizer["notification.notfound"]);

        await _repo.DeleteAsync(item, cancellationToken);

        return Result<Guid>.Success(request.Id);
    }
}