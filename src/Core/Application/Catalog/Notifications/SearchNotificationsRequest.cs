namespace TD.WebApi.Application.Catalog.Notifications;
public class SearchNotificationsRequest : PaginationFilter, IRequest<PaginationResponse<NotificationDto>>
{
    public string? Topic { get; set; }
    public bool? IsRead { get; set; }
    public Guid? CategoryId { get; set; }
}

public class SearchCategoriesRequestHandler : IRequestHandler<SearchNotificationsRequest, PaginationResponse<NotificationDto>>
{
    private readonly IDapperRepository _repository;
    private readonly ICurrentUser _currentUser;


    public SearchCategoriesRequestHandler(IDapperRepository repository, ICurrentUser currentUser) => (_repository, _currentUser) = (repository, currentUser);

    public async Task<PaginationResponse<NotificationDto>> Handle(SearchNotificationsRequest request, CancellationToken cancellationToken)
    {
        string query = @"SELECT Notifications.*, Categories.Name AS CategoryName, Users.FullName AS FullName, Users.UserName AS UserName, Users.Email AS Email, Users.ImageUrl AS ImageUrl, Users.PhoneNumber AS PhoneNumber  ";
        query += @"FROM [Catalog].[Notifications] Notifications 
                LEFT JOIN [Catalog].[Categories] Categories ON Categories.Id = Notifications.[CategoryId]
                LEFT JOIN [Identity].Users ON Users.Id = Notifications.Topic ";

        string where = " ";

        if (request.IsRead.HasValue)
        {

            where += $" AND Notifications.IsRead = {(request.IsRead == true ? 1 : 0)} ";
        }

        if (request.CategoryId.HasValue)
        {

            where += $" AND Notifications.CategoryId = '{request.CategoryId}' ";
        }

        if (!string.IsNullOrEmpty(request.Topic))
        {
            where += $" AND Notifications.Topic = '{request.Topic}' ";
        }

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            where += $" AND (Notifications.Title LIKE N'%{request.Keyword}%' OR Notifications.[Content] LIKE N'%{request.Keyword}%' ) ";
        }

        where = " WHERE Notifications.DeletedOn IS NULL AND Notifications.TenantId = '@tenant' " + where;

        string whereOrder = " ORDER BY Main.CreatedOn DESC ";

        if (request.HasOrderBy())
        {
            string[] orderByFields = request.OrderBy!.Select(field => $"Main.{field}").ToArray();
            whereOrder = " ORDER BY " + string.Join(", ", request.OrderBy!);
        }

        string paging = $" OFFSET {(request.PageNumber - 1) * request.PageSize} ROWS FETCH NEXT {request.PageSize} ROWS ONLY";

        string sql = $"WITH Main AS ({query} {where} ), TotalCount AS (SELECT COUNT(Id) AS [TotalCount]  FROM Main) SELECT * FROM Main, TotalCount {whereOrder} {paging} ";

        return await _repository.PaginatedListNewAsync<NotificationDto>(sql, request.PageNumber, request.PageSize, cancellationToken);
    }
}