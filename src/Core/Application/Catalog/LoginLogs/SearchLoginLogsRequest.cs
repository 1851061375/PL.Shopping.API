namespace TD.WebApi.Application.Catalog.LoginLogs;

public class SearchLoginLogsRequest : PaginationFilter, IRequest<PaginationResponse<LoginLogDto>>
{
    public string? UserName { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class SearchMarketCategoriesRequestHandler : IRequestHandler<SearchLoginLogsRequest, PaginationResponse<LoginLogDto>>
{
    private readonly IReadRepository<LoginLog> _repository;
    private readonly IDapperRepository _repositoryDapper;

    public SearchMarketCategoriesRequestHandler(IReadRepository<LoginLog> repository, IDapperRepository repositoryDapper) => (_repository, _repositoryDapper) = (repository, repositoryDapper);

    public async Task<PaginationResponse<LoginLogDto>> Handle(SearchLoginLogsRequest request, CancellationToken cancellationToken)
    {
        string query = @"SELECT LoginLogs.Id, LoginLogs.CreatedOn, LoginLogs.Ip, LoginLogs.BrowserName, LoginLogs.OperatingSystem, LoginLogs.UserId, LoginLogs.UserName, Users.FullName, Users.ImageUrl 
        FROM [Catalog].[LoginLogs] LoginLogs 
        LEFT JOIN [Identity].[Users] Users ON Users.Id = LoginLogs.UserId  ";
        string where = " ";

        if (request.UserId.HasValue)
        {
            where += $" AND LoginLogs.UserId = '{request.UserId}' ";
        }

        if (request.FromDate.HasValue)
        {
            DateTime date = request.FromDate.Value;

            where += $" AND CAST(CONVERT(date, LoginLogs.CreatedOn) AS datetime2) >= CAST('{date.ToString("yyyy-MM-dd")}' AS datetime2) ";
        }

        if (request.ToDate.HasValue)
        {
            DateTime date = request.ToDate.Value;

            where += $" AND CAST(CONVERT(date, LoginLogs.CreatedOn) AS datetime2) <= CAST('{date.ToString("yyyy-MM-dd")}' AS datetime2) ";
        }

        where = " WHERE LoginLogs.DeletedOn IS NULL  AND LoginLogs.TenantId = '@tenant' " + where;
        string whereOrder = " ORDER BY Main.CreatedOn DESC ";
        string paging = $" OFFSET {(request.PageNumber - 1) * request.PageSize} ROWS FETCH NEXT {request.PageSize} ROWS ONLY";

        string sql = $"WITH Main AS ({query} {where}), TotalCount AS (SELECT COUNT(Id) AS [TotalCount]  FROM Main) SELECT * FROM Main, TotalCount {whereOrder} {paging} ";

        return await _repositoryDapper.PaginatedListNewAsync<LoginLogDto>(sql, request.PageNumber, request.PageSize, cancellationToken);
    }
}