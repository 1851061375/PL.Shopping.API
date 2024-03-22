namespace TD.WebApi.Application.Catalog.Notifications;
public class NotificationDto : IDto
{
    public Guid Id { get; set; }
    public string? Topic { get; set; }
    public string? Title { get; set; } // tiêu đề
    public string? Description { get; set; } // mô tả
    public bool? IsRead { get; set; } = false;
    public string? Data { get; set; }
    public string? Code { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Link { get; set; }
    public string? DeepLink { get; set; }
    public string? CommentId { get; set; }
    public string? DiscussionId { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? ImageUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? Content { get; set; }
    public int? TotalCount { get; set; }
}