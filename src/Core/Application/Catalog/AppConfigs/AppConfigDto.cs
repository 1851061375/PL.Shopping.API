namespace TD.WebApi.Application.Catalog.AppConfigs;

public class AppConfigDto : IDto
{
    public Guid? Id { get; set; }
    public string? Key { get; set; } 
    public string? Value { get; set; } 
    public string? Description { get; set; }
    public bool? IsActivePortal { get; set; }
}