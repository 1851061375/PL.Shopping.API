using System.Reflection;
using System.Runtime.CompilerServices;
using TD.WebApi.Infrastructure.Auth;
using TD.WebApi.Infrastructure.BackgroundJobs;
using TD.WebApi.Infrastructure.Caching;
using TD.WebApi.Infrastructure.Common;
using TD.WebApi.Infrastructure.Cors;
using TD.WebApi.Infrastructure.FileStorage;
using TD.WebApi.Infrastructure.Localization;
using TD.WebApi.Infrastructure.Mailing;
using TD.WebApi.Infrastructure.Mapping;
using TD.WebApi.Infrastructure.Middleware;
using TD.WebApi.Infrastructure.Multitenancy;
using TD.WebApi.Infrastructure.Notifications;
using TD.WebApi.Infrastructure.OpenApi;
using TD.WebApi.Infrastructure.Persistence;
using TD.WebApi.Infrastructure.Persistence.Initialization;
using TD.WebApi.Infrastructure.SecurityHeaders;
using TD.WebApi.Infrastructure.Validations;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TD.WebApi.Infrastructure.Minio;
using TD.WebApi.Infrastructure.Ldap;
using TD.WebApi.Infrastructure.SyncfusionReport;
using Asp.Versioning;
using TD.WebApi.Infrastructure.VietQR;

[assembly: InternalsVisibleTo("Infrastructure.Test")]

namespace TD.WebApi.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var applicationAssembly = typeof(TD.WebApi.Application.Startup).GetTypeInfo().Assembly;
        MapsterSettings.Configure();
        return services
            .AddApiVersioning()
            .AddAuth(config)
            .AddBackgroundJobs(config)
            .AddCaching(config)
            .AddCorsPolicy(config)
            .AddSyncfusionReport(config)
            .AddExceptionMiddleware()
            .AddBehaviours(applicationAssembly)
            .AddHealthCheck()
            .AddPOLocalization(config)
            .AddMailing(config)
            .AddTDMinio(config)
            .AddVietQR(config)
            .AddLdap(config)
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddMultitenancy()
            .AddNotifications(config)
            .AddOpenApiDocumentation(config)
            .AddPersistence()
            .AddRequestLogging(config)
            .AddRouting(options => options.LowercaseUrls = true)
            .AddServices();
    }

    private static IServiceCollection AddApiVersioning(this IServiceCollection services) =>
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        }).Services;

    private static IServiceCollection AddHealthCheck(this IServiceCollection services) =>
        services.AddHealthChecks().AddCheck<TenantHealthCheck>("Tenant").Services;

    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
            .InitializeDatabasesAsync(cancellationToken);
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config) =>
        builder
            .UseRequestLocalization()
            .UseStaticFiles()
            .UseSecurityHeaders(config)
            .UseFileStorage()
            .UseExceptionMiddleware()
            .UseRouting()
            .UseCorsPolicy()
            .UseAuthentication()
            .UseCurrentUser()
            .UseMultiTenancy()
            .UseAuthorization()
            .UseRequestLogging(config)
            .UseHangfireDashboard(config)
            .UseOpenApiDocumentation(config);

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapControllers().RequireAuthorization();
        builder.MapHealthCheck();
        builder.MapNotifications();
        return builder;
    }

    private static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapHealthChecks("/api/health");
}