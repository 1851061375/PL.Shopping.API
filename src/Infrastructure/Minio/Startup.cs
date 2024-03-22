using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace TD.WebApi.Infrastructure.Minio;

internal static class Startup
{
    internal static IServiceCollection AddTDMinio(this IServiceCollection services, IConfiguration config)
    {
        var minioSettings = config.GetSection(nameof(MinioSettings)).Get<MinioSettings>();
        services.Configure<MinioSettings>(config.GetSection(nameof(MinioSettings)));
        services.AddMinio(configureClient => configureClient.WithEndpoint(minioSettings.Endpoint).WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey));
        return services;
    }
}