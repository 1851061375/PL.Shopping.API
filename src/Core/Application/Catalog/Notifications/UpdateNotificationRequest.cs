namespace TD.WebApi.Application.Catalog.Notifications;
public class UpdateNotificationRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public bool? IsRead { get; set; }
}

public class UpdateNotificationRequestValidator : CustomValidator<UpdateNotificationRequest>
{
    public UpdateNotificationRequestValidator(IRepository<Notification> repository, IStringLocalizer<UpdateNotificationRequestValidator> localizer) =>
        RuleFor(p => p.IsRead)
            .NotEmpty();
}

public class UpdateNotificationRequestHandler : IRequestHandler<UpdateNotificationRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<Notification> _repository;
    private readonly IStringLocalizer<UpdateNotificationRequestHandler> _localizer;

    public UpdateNotificationRequestHandler(IRepositoryWithEvents<Notification> repository, IStringLocalizer<UpdateNotificationRequestHandler> localizer) =>
        (_repository, _localizer) = (repository, localizer);

    public async Task<Result<Guid>> Handle(UpdateNotificationRequest request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = item ?? throw new NotFoundException(string.Format(_localizer["notification.notfound"], request.Id));

        item.Update(request.IsRead);

        await _repository.UpdateAsync(item, cancellationToken);

        return Result<Guid>.Success(request.Id);
    }
}