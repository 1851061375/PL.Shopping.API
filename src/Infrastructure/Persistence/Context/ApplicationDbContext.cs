using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TD.WebApi.Application.Common.Events;
using TD.WebApi.Application.Common.Interfaces;
using TD.WebApi.Domain.Catalog;
using TD.WebApi.Infrastructure.Persistence.Configuration;

namespace TD.WebApi.Infrastructure.Persistence.Context;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(ITenantInfo currentTenant, DbContextOptions options, ICurrentUser currentUser, ISerializerService serializer, IOptions<DatabaseSettings> dbSettings, IEventPublisher events)
        : base(currentTenant, options, currentUser, serializer, dbSettings, events)
    {
    }

    #region Catalog

    public DbSet<AppConfig> AppConfigs => Set<AppConfig>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategoryGroup> CategoryGroups => Set<CategoryGroup>();
    public DbSet<TokuteiOrder> TokuteiOrders => Set<TokuteiOrder>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaNames.Catalog);
    }
}