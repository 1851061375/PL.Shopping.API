using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TD.WebApi.Infrastructure.VietQR;

internal static class Startup
{
    internal static IServiceCollection AddVietQR(this IServiceCollection services, IConfiguration config) =>
        services.Configure<VietQRSettings>(config.GetSection(nameof(VietQRSettings)));
}