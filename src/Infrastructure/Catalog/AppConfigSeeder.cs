using System.Reflection;
using TD.WebApi.Application.Common.Interfaces;
using TD.WebApi.Infrastructure.Persistence.Context;
using TD.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.Extensions.Logging;
using TD.WebApi.Domain.Catalog;

namespace TD.WebApi.Infrastructure.Catalog;

public class AppConfigSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<AppConfigSeeder> _logger;

    public AppConfigSeeder(ISerializerService serializerService, ILogger<AppConfigSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!_db.AppConfigs.Any())
        {
            _logger.LogInformation("Started to Seed AppConfigs.");

            // Here you can use your own logic to populate the database.
            // As an example, I am using a JSON file to populate the database.
            string brandData = await File.ReadAllTextAsync(path + "/Catalog/appconfigs.json", cancellationToken);
            var datas = _serializerService.Deserialize<List<AppConfig>>(brandData);

            if (datas != null)
            {
                foreach (var item in datas)
                {
                    await _db.AppConfigs.AddAsync(item, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded JobTitles.");
        }
    }
}