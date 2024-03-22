using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TD.WebApi.Infrastructure.SyncfusionReport;

internal static class Startup
{
    internal static IServiceCollection AddSyncfusionReport(this IServiceCollection services, IConfiguration config)
    {
        //var syncfusionSettings = config.GetSection(nameof(SyncfusionSettings)).Get<SyncfusionSettings>();
        //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionSettings.LicenseKey);
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBMAY9C3t2VlhhQlVEfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn9Td0ZjUHpfc3JQR2lb");
        return services;
    }
}