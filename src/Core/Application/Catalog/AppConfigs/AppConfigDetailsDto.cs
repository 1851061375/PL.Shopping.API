namespace TD.WebApi.Application.Catalog.AppConfigs;

public class AppConfigDetailsDto : IDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? Description { get; set; }
    public bool? IsActivePortal { get; set; }
}