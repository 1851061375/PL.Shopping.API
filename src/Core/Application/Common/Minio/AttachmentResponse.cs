namespace TD.WebApi.Application.Common.Minio;

public class AttachmentResponse
{
    public string Name { get; set; }
    public string? Type { get; set; }
    public string? Url { get; set; }
    public decimal? Size { get; set; }
}