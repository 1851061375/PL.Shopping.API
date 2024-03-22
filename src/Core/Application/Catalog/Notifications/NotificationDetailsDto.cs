namespace TD.WebApi.Application.Catalog.Notifications;
public class NotificationDetailsDto : IDto
{
    public Guid Id { get; set; }
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
    public DateTime? CreatedOn { get; set; }
}