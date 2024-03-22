namespace TD.WebApi.Application.Identity.Users;

public class UserListFilter : PaginationFilter
{
    public int? Type { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsVerified { get; set; }
    public string? Gender { get; set; }
    public bool? IsSync { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid[]? NotInIds { get; set; }
    public int[]? Types { get; set; }
}