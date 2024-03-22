namespace TD.WebApi.Domain.Catalog;

/// <summary>
/// Cấu hình mẫu mail gửi
/// </summary>
public class MailConfiguration : AuditableEntity, IAggregateRoot
{
    public string Key { get; set; } = default!;
    public string? Name { get; set; }
    /// <summary>
    /// Chủ đề mail
    /// </summary>
    public string? Subject { get; set; }
    public string? Description { get; set; }
    /// <summary>
    /// Nội dung email
    /// </summary>
    public string? Content { get; set; }
    public bool? IsActive { get; set; } = true;

    public MailConfiguration(string key, string? name, string? subject, string? description, string? content, bool? isActive)
    {
        Key = key;
        Name = name;
        Subject = subject;
        Description = description;
        Content = content;
        IsActive = isActive;
    }

    public MailConfiguration Update(string key, string? name, string? subject, string? description, string? content, bool? isActive)
    {
        Key = key;
        Name = name;
        Subject = subject;
        Description = description;
        Content = content;
        IsActive = isActive;
        return this;
    }
}