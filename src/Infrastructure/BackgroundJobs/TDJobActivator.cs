using Finbuckle.MultiTenant;
using TD.WebApi.Infrastructure.Auth;
using TD.WebApi.Infrastructure.Common;
using TD.WebApi.Infrastructure.Multitenancy;
using TD.WebApi.Shared.Multitenancy;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;

namespace TD.WebApi.Infrastructure.BackgroundJobs;

public class TDJobActivator : JobActivator
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TDJobActivator(IServiceScopeFactory scopeFactory) =>
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

    public override JobActivatorScope BeginScope(PerformContext context) =>
        new Scope(context, _scopeFactory.CreateScope());

    private class Scope : JobActivatorScope, IServiceProvider
    {
        private readonly PerformContext _context;
        private readonly IServiceScope _scope;

        public Scope(PerformContext context, IServiceScope scope)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));

            ReceiveParameters();
        }

        private void ReceiveParameters()
        {

            string tenantId = _context.GetJobParameter<string>(MultitenancyConstants.TenantIdName);
            if (tenantId is not null)
            {
                var tenantContext = _scope.ServiceProvider.GetRequiredService<TenantDbContext>();
                var tenantInfo = tenantContext.TenantInfo.Find(tenantId);

                if (tenantInfo is null)
                {
                    throw new InvalidOperationException("Tenant is not valid");
                }

                _scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor>()
                    .MultiTenantContext = new MultiTenantContext<TDTenantInfo>
                    {
                        TenantInfo = tenantInfo
                    };
            }


            /*var tenantInfo = _context.GetJobParameter<TDTenantInfo>(MultitenancyConstants.TenantIdName);
            if (tenantInfo is not null)
            {
                _scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor>()
                    .MultiTenantContext = new MultiTenantContext<TDTenantInfo>
                    {
                        TenantInfo = tenantInfo
                    };
            }*/

            string userId = _context.GetJobParameter<string>(QueryStringKeys.UserId);
            if (!string.IsNullOrEmpty(userId))
            {
                _scope.ServiceProvider.GetRequiredService<ICurrentUserInitializer>()
                    .SetCurrentUserId(userId);
            }
        }

        public override object Resolve(Type type) =>
            ActivatorUtilities.GetServiceOrCreateInstance(this, type);

        object? IServiceProvider.GetService(Type serviceType) =>
            serviceType == typeof(PerformContext)
                ? _context
                : _scope.ServiceProvider.GetService(serviceType);
    }
}