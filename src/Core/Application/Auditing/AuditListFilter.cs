namespace TD.WebApi.Application.Auditing;

public class AuditListFilter : PaginationFilter
{
    public Guid? UserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}