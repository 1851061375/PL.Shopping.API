namespace TD.WebApi.Domain.Catalog;

/// <summary>
/// Cấu hình hệ thống
/// </summary>
public class AppConfig : AuditableEntity, IAggregateRoot
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string? Description { get; set; }
    public bool? IsActivePortal { get; set; }

    public AppConfig(string key, string value, string? description, bool? isActivePortal)
    {
        Key = key;
        Value = value;
        Description = description;
        IsActivePortal = isActivePortal;
    }

    public AppConfig Update(string? key, string? value, string? description, bool? isActivePortal)
    {
        if (key is not null && Key?.Equals(key) is not true) Key = key;
        if (value is not null && Value?.Equals(value) is not true) Value = value;
        if (isActivePortal is not null && IsActivePortal?.Equals(isActivePortal) is not true) IsActivePortal = isActivePortal;
        if (description is not null && Description?.Equals(description) is not true) Description = description;

        return this;
    }
}