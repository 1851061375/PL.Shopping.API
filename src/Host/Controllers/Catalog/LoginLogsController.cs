using TD.WebApi.Application.Catalog.LoginLogs;

namespace TD.WebApi.Host.Controllers.Catalog;

public class LoginLogsController : VersionedApiController
{
    [HttpPost("search")]
    [OpenApiOperation("Danh sách cấu hình hệ thống.", "")]
    public Task<PaginationResponse<LoginLogDto>> SearchAsync(SearchLoginLogsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Chi tiết cấu hình hệ thống.", "")]
    public Task<Result<LoginLogDetailsDto>> GetAsync(Guid id)
    {
        return Mediator.Send(new GetLoginLogRequest(id));
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Xóa cấu hình hệ thống.", "")]
    public Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteLoginLogRequest(id));
    }
}