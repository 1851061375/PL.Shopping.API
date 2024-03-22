namespace TD.WebApi.Application.Catalog.LoginLogs;

public class LoginLogDto : IDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? UserId { get; set; }
    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
    public string? BrowserName { get; set; }
    public string? OperatingSystem { get; set; }
    public string? Type { get; set; }
    public DateTime? CreatedOn { get; set; }
    public int? TotalCount { get; set; }
}