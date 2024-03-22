namespace TD.WebApi.Application.Auditing;

public interface IAuditService : ITransientService
{
    Task<List<AuditDto>> GetUserTrailsAsync(Guid userId);
    Task<PaginationResponse<AuditDto>> SearchAsync(AuditListFilter filter, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid? id, CancellationToken cancellationToken);
}