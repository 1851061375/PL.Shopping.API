namespace TD.WebApi.Application.Catalog.MailConfigurations;

public class MailConfigurationByKeySpec : Specification<MailConfiguration>, ISingleResultSpecification
{
    public MailConfigurationByKeySpec(string key) =>
        Query.Where(b => b.Key == key);
}