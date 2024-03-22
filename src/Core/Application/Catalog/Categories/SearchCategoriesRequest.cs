using TD.WebApi.Application.Common.Persistence;

namespace TD.WebApi.Application.Catalog.Categories;

public class SearchCategoriesRequest : PaginationFilter, IRequest<PaginationResponse<CategoryDto>>
{
    public bool? IsActive { get; set; }
    public Guid? CategoryGroupId { get; set; }
    public string? CategoryGroupCode { get; set; }

}

public class SearchCategorysRequestHandler : IRequestHandler<SearchCategoriesRequest, PaginationResponse<CategoryDto>>
{
    private readonly IDapperRepository _repository;


    public SearchCategorysRequestHandler(IDapperRepository repository) => _repository = repository;

    public async Task<PaginationResponse<CategoryDto>> Handle(SearchCategoriesRequest request, CancellationToken cancellationToken)
    {
        string query = @"SELECT Categories.*, CategoryGroups.Name AS CategoryGroupName, CategoryGroups.Code AS CategoryGroupCode ";
        query += @"FROM [Catalog].[Categories] Categories
            LEFT JOIN [Catalog].[CategoryGroups] CategoryGroups ON CategoryGroups.Id = Categories.CategoryGroupId ";

        string where = " ";

        if (request.IsActive.HasValue)
        {
            where += $" AND Categories.IsActive = {(request.IsActive == true ? 1 : 0)} ";
        }

        if (request.CategoryGroupId.HasValue)
        {
            where += $" AND Categories.CategoryGroupId = '{request.CategoryGroupId}' ";
        }

        if (!string.IsNullOrEmpty(request.CategoryGroupCode))
        {
            where += $" AND CategoryGroups.Code = '{request.CategoryGroupCode}' ";
        }

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            where += $" AND (Categories.Name LIKE N'%{request.Keyword}%' OR Categories.Code LIKE '%{request.Keyword}%' ) ";
        }

        where = " WHERE Categories.DeletedOn IS NULL AND Categories.TenantId = '@tenant' " + where;

        string whereOrder = " ORDER BY Main.SortOrder ";

        string paging = $" OFFSET {(request.PageNumber - 1) * request.PageSize} ROWS FETCH NEXT {request.PageSize} ROWS ONLY";

        string sql = $"WITH Main AS ({query} {where} ), TotalCount AS (SELECT COUNT(Id) AS [TotalCount]  FROM Main) SELECT * FROM Main, TotalCount {whereOrder} {paging} ";

        return await _repository.PaginatedListNewAsync<CategoryDto>(sql, request.PageNumber, request.PageSize, cancellationToken);
    }
}