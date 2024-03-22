namespace TD.WebApi.Domain.Catalog;

/// <summary>
/// Thông báo của cá nhân người dùng
/// </summary>
public class Notification : AuditableEntity, IAggregateRoot
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

    public Notification(string? topic, string? title, string? description, string? content, bool? isRead, string? data, string? code, DefaultIdType? categoryId, string? link, string? deepLink, string? commentId, string? discussionId)
    {
        Topic = topic;
        Title = title;
        Description = description;
        Content = content;
        IsRead = isRead;
        Data = data;
        Code = code;
        CategoryId = categoryId;
        Link = link;
        DeepLink = deepLink;
        CommentId = commentId;
        DiscussionId = discussionId;
    }

    public Notification Update(string? topic, string? title, string? description, string? content, bool? isRead, string? data, string? code, DefaultIdType? categoryId, string? link, string? deepLink, string? commentId, string? discussionId)
    {
        Topic = topic;
        Title = title;
        Description = description;
        Content = content;
        IsRead = isRead;
        Data = data;
        Code = code;
        CategoryId = categoryId;
        Link = link;
        DeepLink = deepLink;
        CommentId = commentId;
        DiscussionId = discussionId;
        return this;
    }

    public Notification Update(bool? isRead)
    {
        if (isRead is not null && IsRead?.Equals(isRead) is not true) IsRead = isRead;
        return this;
    }
}