namespace TD.WebApi.Application.Catalog.Notifications;
public class CreateNotificationRequest : IRequest<Result<Guid>>
{
    public string? Topic { get; set; }
    public string? Title { get; set; } // tiêu đề
    public string? Description { get; set; } // mô tả
    public string? Content { get; set; }
    public bool? IsRead { get; set; } = false;
    public string? Data { get; set; }
    public string? Code { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Link { get; set; }
    public string? DeepLink { get; set; }
    public string? CommentId { get; set; }
    public string? DiscussionId { get; set; }
}

public class CreateCategoryRequestHandler : IRequestHandler<CreateNotificationRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<Notification> _repository;

    public CreateCategoryRequestHandler(IRepositoryWithEvents<Notification> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateNotificationRequest request, CancellationToken cancellationToken)
    {
        var item = new Notification(request.Topic, request.Title, request.Description, request.Content, request.IsRead, request.Data, request.Code, request.CategoryId, request.Link, request.DeepLink, request.CommentId, request.DiscussionId);

        await _repository.AddAsync(item, cancellationToken);

        return Result<Guid>.Success(item.Id);
    }
}