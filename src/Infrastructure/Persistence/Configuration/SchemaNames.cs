namespace TD.WebApi.Infrastructure.Persistence.Configuration;

internal static class SchemaNames
{
    // TODO: figure out how to capitalize these only for Oracle
    public static string Auditing = nameof(Auditing); // "AUDITING";
    public static string Catalog = nameof(Catalog); // "CATALOG";
    public static string EmployeeInfo = nameof(EmployeeInfo); // "CATALOG";
    public static string TimeSheet = nameof(TimeSheet); // "CATALOG";
    public static string Building = nameof(Building); // "CATALOG";
    public static string Identity = nameof(Identity); // "IDENTITY";
    public static string MultiTenancy = nameof(MultiTenancy); // "MULTITENANCY";
}