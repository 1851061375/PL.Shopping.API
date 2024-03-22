using TD.WebApi.Application.Catalog.TokuteiOrders;

namespace TD.WebApi.Host.Controllers.Catalog;

public class TokuteiOrdersController : VersionedApiController
{
    [HttpPost("search")]
    [OpenApiOperation("Danh sách Đơn Tokutei.", "")]
    public Task<PaginationResponse<TokuteiOrderDto>> SearchAsync(SearchTokuteiOrderRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Chi tiết Đơn Tokutei.", "")]
    public Task<Result<TokuteiOrderDto>> GetAsync(Guid id)
    {
        return Mediator.Send(new GetTokuteiOrderRequest(id));
    }

    [HttpPost]
    [MustHavePermission(TDAction.Manage, TDResource.TokuteiOrders)]
    [OpenApiOperation("Tạo mới Đơn Tokutei.", "")]
    public Task<Result<Guid>> CreateAsync(CreateTokuteiOrderRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(TDAction.Manage, TDResource.TokuteiOrders)]
    [OpenApiOperation("Cập nhật Đơn Tokutei.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateTokuteiOrderRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(TDAction.Manage, TDResource.TokuteiOrders)]
    [OpenApiOperation("Xóa Đơn Tokutei.", "")]
    public Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteTokuteiOrderRequest(id));
    }

}