namespace TD.WebApi.Application.Catalog.Users;

public class SearchUsersRequest : PaginationFilter, IRequest<PaginationResponse<UserDto>>
{
    public bool? IsActive { get; set; }
    public Guid? BranchId { get; set; }
}

public class SearchBanksRequestHandler : IRequestHandler<SearchUsersRequest, PaginationResponse<UserDto>>
{
    private readonly IDapperRepository _repository;
    public SearchBanksRequestHandler(IDapperRepository repository) => _repository = repository;

    public async Task<PaginationResponse<UserDto>> Handle(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        /* string query = @"SELECT Users.*,  Branches.Name AS BranchName FROM [Identity].[Users] AS Users
        LEFT JOIN [Catalog].[Branches] AS Branches ON Branches.Id = Users.BranchId
 ";

         string queryCount = $"SELECT COUNT(*) FROM [Identity].[Users] AS Users  LEFT JOIN [Catalog].[Branches] AS Branches ON Branches.Id = Users.BranchId " +
             @"  ";

         string where = " ";

         if (request.BranchId.HasValue)
         {
             where += $" AND Users.BranchId = '{request.BranchId}' ";
         }

         if (request.IsActive.HasValue && request.IsActive == true)
         {
             where += $" AND Users.IsActive = 1 ";
         }

         if (request.IsActive.HasValue && request.IsActive == false)
         {
             where += $" AND  (Users.IsActive = 0) ";
         }


         where = " WHERE Users.TenantId = '@tenant' " + where;

         string? ordering = ConvertBack(request.OrderBy);
         string whereOrder = !string.IsNullOrWhiteSpace(ordering) ? $" ORDER BY {ordering} " : " ORDER BY Users.CreatedOn DESC ";

         string paging = $" OFFSET {(request.PageNumber - 1) * request.PageSize} ROWS FETCH NEXT {request.PageSize} ROWS ONLY";

         string sql = $"{query} {where} {whereOrder} {paging} " + $"{queryCount} {where}";

         return await _repository.PaginatedListAsync<UserDto>(sql, request.PageNumber, request.PageSize, cancellationToken);*/
        string whereInBranch = "";

        if (request.BranchId.HasValue)
        {
            whereInBranch += $"  WHERE Br.Id = '{request.BranchId}' OR Br.FullParentIds LIKE CONCAT ('%', '{request.BranchId}','%')";
        }

        string query = @"SELECT Users.*,
       Branches.Name AS BranchName
FROM [Identity].[Users] AS Users 
              LEFT JOIN (
                     SELECT Id,
                            Name,
                            Code, FullParentIds 
                     FROM [Catalog].[Branches] AS Br 
                     " + whereInBranch + @"
              ) AS Branches ON Branches.Id = Users.BranchId ";

        string groupby = @"";

        string where = " ";
        if (request.IsActive.HasValue && request.IsActive == true)
        {
            where += $" AND Users.IsActive = 1 ";
        }

        if (request.IsActive.HasValue && request.IsActive == false)
        {
            where += $" AND  (Users.IsActive = 0) ";
        }

        if (request.BranchId.HasValue)
        {
            where += $"  AND  Branches.Id = '{request.BranchId}' OR Branches.FullParentIds LIKE CONCAT ('%', '{request.BranchId}','%')";
        }

        where = " WHERE Users.TenantId = '@tenant' " + where;

        string? ordering = ConvertBack(request.OrderBy);
        string whereOrder = " ORDER BY Main.CreatedOn DESC ";

        string paging = $" OFFSET {(request.PageNumber - 1) * request.PageSize} ROWS FETCH NEXT {request.PageSize} ROWS ONLY";

        string sql = $"WITH Main AS ({query} {where} {groupby}), TotalCount AS (SELECT COUNT(ID) AS [TotalCount]  FROM Main) SELECT * FROM Main, TotalCount {whereOrder} {paging} ";

        return await _repository.PaginatedListNewAsync<UserDto>(sql, request.PageNumber, request.PageSize, cancellationToken);
    }

    public string? ConvertBack(string[]? item)
    {
        return item?.Any() == true ? string.Join(",", item) : null;
    }
}