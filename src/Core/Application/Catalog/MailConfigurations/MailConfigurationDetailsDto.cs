namespace TD.WebApi.Application.Catalog.MailConfigurations;

public class MailConfigurationDetailsDto : IDto
{
    public Guid Id { get; set; }
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
}