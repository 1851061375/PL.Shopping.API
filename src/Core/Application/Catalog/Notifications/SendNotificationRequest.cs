namespace TD.WebApi.Application.Catalog.Notifications;

public class SendNotificationRequest : IRequest<string>
{
    public List<string> Topics { get; set; } = default!;
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

public class SendNotificationHandler : IRequestHandler<SendNotificationRequest, string>
{
    private readonly IJobService _jobService;

    public SendNotificationHandler(IJobService jobService) => _jobService = jobService;

    public Task<string> Handle(SendNotificationRequest request, CancellationToken cancellationToken)
    {
        string jobId = _jobService.Enqueue<ISendNotificationJob>(x => x.SendNotificationAsync(request, default));
        return Task.FromResult(jobId);
    }
}