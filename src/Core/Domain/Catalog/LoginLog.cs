namespace TD.WebApi.Domain.Catalog;

/// <summary>
/// Từ khóa
/// </summary>
public class LoginLog : AuditableEntity, IAggregateRoot
{
    public string? UserName { get;  set; }
    public string? FullName { get; set; }
    public Guid? UserId { get;  set; }
    public string? Ip { get;  set; }
    public string? UserAgent { get; set; }
    public string? BrowserName { get; set; }
    public string? OperatingSystem { get; set; }
    public string? Type { get; set; }

    public LoginLog(string? userName, string? fullName, DefaultIdType? userId, string? ip, string? userAgent, string? browserName, string? operatingSystem, string? type)
    {
        UserName = userName;
        FullName = fullName;
        UserId = userId;
        Ip = ip;
        UserAgent = userAgent;
        BrowserName = browserName;
        OperatingSystem = operatingSystem;
        Type = type;
    }
}