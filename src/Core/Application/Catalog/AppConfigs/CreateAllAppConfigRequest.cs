namespace TD.WebApi.Application.Catalog.AppConfigs;

public partial class CreateAllAppConfigRequest : IRequest<Result<bool>>
{
   public ICollection<ConfigApp>? Data { get; set; }
}

public class ConfigApp
{
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? Description { get; set; }
    public bool? IsActivePortal { get; set; }
}

public class CreateAllAppConfigRequestHandler : IRequestHandler<CreateAllAppConfigRequest, Result<bool>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<AppConfig> _repository;

    public CreateAllAppConfigRequestHandler(IRepositoryWithEvents<AppConfig> repository) => _repository = repository;

    public async Task<Result<bool>> Handle(CreateAllAppConfigRequest request, CancellationToken cancellationToken)
    {

        if (request.Data != null && request.Data.Any())
        {
            foreach (var i in request.Data)
            {
                var item = await _repository.FirstOrDefaultAsync(new AppConfigByNameSpec(i.Key), cancellationToken);
                if (item != null)
                {
                    item.Update(i.Key, i.Value, i.Description, i.IsActivePortal);
                    await _repository.UpdateAsync(item, cancellationToken);
                } else
                {
                    item = new AppConfig(i.Key, i.Value, i.Description, i.IsActivePortal);
                    await _repository.AddAsync(item, cancellationToken);
                }
            }
        }

        return Result<bool>.Success(true);
    }
}