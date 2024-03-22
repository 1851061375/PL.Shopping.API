namespace TD.WebApi.Domain.Catalog;

/// <summary>
/// Tệp đính kèm
/// </summary>
public class Attachment : AuditableEntity, IAggregateRoot
{
    public string Name { get;  set; }
    public string? Extension { get; set; }
    public decimal? FileSize { get; set; }
    public bool? IsAllowToDownload { get; set; }
    public string? Description { get;  set; }
    public string? FileURL { get; set; }

    public Attachment(string name, string? extension, decimal? fileSize, bool? isAllowToDownload, string? description, string? fileURL)
    {
        Name = name;
        Extension = extension;
        FileSize = fileSize;
        IsAllowToDownload = isAllowToDownload;
        Description = description;
        FileURL = fileURL;
    }
}