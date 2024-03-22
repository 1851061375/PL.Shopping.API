namespace TD.WebApi.Application.Catalog.MailConfigurations;

public class MailConfigurationDto : IDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; } = true;
    public DateTime? CreatedOn { get; set; }
}