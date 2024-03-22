using Finbuckle.MultiTenant;
using TD.WebApi.Application.Common.Interfaces;
using TD.WebApi.Infrastructure.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TD.WebApi.Infrastructure.BackgroundJobs.RecurringJobs;

public class RecurringJobInitialization : IRecurringJobInitialization
{
    private readonly ILogger<RecurringJobInitialization> _logger;
    private readonly IJobService _jobService;
    private readonly TenantDbContext _tenantDbContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITenantInfo _tenantInfo;

    public RecurringJobInitialization(ILogger<RecurringJobInitialization> logger, IJobService jobService, TenantDbContext tenantDbContext, IServiceProvider serviceProvider, ITenantInfo tenantInfo)
    {
        _logger = logger;
        _jobService = jobService;
        _tenantDbContext = tenantDbContext;
        _serviceProvider = serviceProvider;
        _tenantInfo = tenantInfo;
    }

    public async Task InitializeJobsForTenantAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Hangfire: Initializing Recurring Jobs");
        foreach (var tenant in await _tenantDbContext.TenantInfo.ToListAsync(cancellationToken))
        {
            InitializeJobsForTenant(tenant);
        }
    }

    public void InitializeJobsForTenant(TDTenantInfo tenant)
    {
        // First create a new scope
        using var scope = _serviceProvider.CreateScope();

        // Then set current tenant so the right connectionstring is used
        scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor>()
            .MultiTenantContext = new MultiTenantContext<TDTenantInfo>()
            {
                TenantInfo = tenant
            };

        scope.ServiceProvider.GetRequiredService<IRecurringJobInitialization>()
            .InitializeRecurringJobs(tenant.Identifier);
    }

    public void InitializeRecurringJobs(string tenantId)
    {
       /* _jobService.AddOrUpdate<IFetchDataJob>($"{tenantId}-ProductOrdersJob", x => x.FetchDataEventEvery5Minute(default), () => Cron.Minutely(), TimeZoneInfo.Utc, "default");*/
        _logger.LogInformation($"{tenantId}: All recurring jobs have been initialized.");
    }
}