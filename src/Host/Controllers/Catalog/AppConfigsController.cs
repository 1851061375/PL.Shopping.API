using TD.WebApi.Application.Catalog.AppConfigs;

namespace TD.WebApi.Host.Controllers.Catalog;

public class AppConfigsController : VersionedApiController
{
    [HttpPost("search")]
    [OpenApiOperation("Danh sách cấu hình hệ thống.", "")]
    public Task<PaginationResponse<AppConfigDto>> SearchAsync(SearchAppConfigsRequest request)
    {
        return Mediator.Send(request);
    }


    [HttpGet("{id:guid}")]
    [OpenApiOperation("Chi tiết cấu hình hệ thống.", "")]
    public Task<Result<AppConfigDetailsDto>> GetAsync(Guid id)
    {
        return Mediator.Send(new GetAppConfigRequest(id));
    }

    [HttpPost]
    [MustHavePermission(TDAction.Manage, TDResource.System)]
    [OpenApiOperation("Tạo mới cấu hình hệ thống.", "")]
    public Task<Result<Guid>> CreateAsync(CreateAppConfigRequest request)
    {
        return Mediator.Send(request);
    }


    [HttpPost("createall")]
    [OpenApiOperation("Tạo mới cấu hình hệ thống.", "")]
    [MustHavePermission(TDAction.Manage, TDResource.System)]
    public Task<Result<bool>> CreateAllAsync(CreateAllAppConfigRequest request)
    {
        return Mediator.Send(request);
    }


    [HttpPut("{id:guid}")]
    [MustHavePermission(TDAction.Manage, TDResource.System)]
    [OpenApiOperation("Cập nhật cấu hình hệ thống.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateAppConfigRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(TDAction.Manage, TDResource.System)]
    [OpenApiOperation("Xóa cấu hình hệ thống.", "")]
    public Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteAppConfigRequest(id));
    }
}