namespace TD.WebApi.Application.Catalog.AppConfigs;

public class AppConfigByNameSpec : Specification<AppConfig>, ISingleResultSpecification
{
    public AppConfigByNameSpec(string key) =>
        Query.Where(b => b.Key == key);
}