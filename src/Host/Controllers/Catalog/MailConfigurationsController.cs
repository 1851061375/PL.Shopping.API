using TD.WebApi.Application.Catalog.MailConfigurations;

namespace TD.WebApi.Host.Controllers.Catalog;

public class MailConfigurationsController : VersionedApiController
{
    [HttpPost("search")]
    [OpenApiOperation("Danh sách Cấu hình email.", "")]
    public Task<PaginationResponse<MailConfigurationDto>> SearchAsync(SearchMailConfigurationsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Chi tiết Cấu hình email.", "")]
    public Task<Result<MailConfigurationDetailsDto>> GetAsync(Guid id)
    {
        return Mediator.Send(new GetMailConfigurationRequest(id));
    }

    [HttpPost]
    [OpenApiOperation("Tạo mới Cấu hình email.", "")]
    [MustHavePermission(TDAction.Manage, TDResource.Email)]
    public Task<Result<Guid>> CreateAsync(CreateMailConfigurationRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [OpenApiOperation("Cập nhật Cấu hình email.", "")]
    [MustHavePermission(TDAction.Manage, TDResource.Email)]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateMailConfigurationRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Xóa Cấu hình email.", "")]
    [MustHavePermission(TDAction.Manage, TDResource.Email)]
    public Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteMailConfigurationRequest(id));
    }

}