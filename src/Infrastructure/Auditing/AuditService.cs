using TD.WebApi.Application.Auditing;
using TD.WebApi.Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Ardalis.Specification.EntityFrameworkCore;
using TD.WebApi.Application.Common.Exceptions;
using TD.WebApi.Application.Common.Models;
using TD.WebApi.Application.Common.Persistence;

namespace TD.WebApi.Infrastructure.Auditing;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly IDapperRepository _repository;

    public AuditService(ApplicationDbContext context, IDapperRepository repository) => (_context, _repository) = (context, repository);

    public async Task<List<AuditDto>> GetUserTrailsAsync(Guid userId)
    {
        var trails = await _context.AuditTrails
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.DateTime)
            .Take(250)
            .ToListAsync();

        return trails.Adapt<List<AuditDto>>();
    }

    public async Task<PaginationResponse<AuditDto>> SearchAsync(AuditListFilter filter, CancellationToken cancellationToken)
    {

        string query = @"SELECT AuditTrails.* , Users.FullName AS FullName, Users.ImageUrl AS ImageUrl, Users.UserName AS UserName 
        FROM [Auditing].[AuditTrails] AuditTrails
        LEFT JOIN [Identity].[Users] Users ON Users.Id = AuditTrails.UserId ";

        string where = " ";

        if (filter.UserId.HasValue)
        {

            where += $" AND AuditTrails.UserId = '{filter.UserId}' ";
        }

        if (filter.FromDate.HasValue)
        {
            DateTime date = filter.FromDate.Value;

            where += $" AND CAST(CONVERT(date, AuditTrails.DateTime) AS datetime2) >= CAST('{date.ToString("yyyy-MM-dd")}' AS datetime2) ";
        }

        if (filter.ToDate.HasValue)
        {
            DateTime date = filter.ToDate.Value;

            where += $" AND CAST(CONVERT(date, AuditTrails.DateTime) AS datetime2) <= CAST('{date.ToString("yyyy-MM-dd")}' AS datetime2) ";
        }

        where = " WHERE AuditTrails.TenantId = '@tenant' " + where;
        string whereOrder = " ORDER BY Main.DateTime DESC ";

        string paging = $" OFFSET {(filter.PageNumber - 1) * filter.PageSize} ROWS FETCH NEXT {filter.PageSize} ROWS ONLY";

        string sql = $"WITH Main AS ({query} {where}), TotalCount AS (SELECT COUNT(Id) AS [TotalCount]  FROM Main) SELECT * FROM Main, TotalCount {whereOrder} {paging} ";

        return await _repository.PaginatedListNewAsync<AuditDto>(sql, filter.PageNumber, filter.PageSize, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid? id, CancellationToken cancellationToken)
    {
        var item = await _context.AuditTrails
           .AsNoTracking()
           .Where(u => u.Id == id)
           .FirstOrDefaultAsync(cancellationToken);

        _ = item ?? throw new NotFoundException("Audit Not Found.");

        _context.AuditTrails.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);

        return true;

    }
}