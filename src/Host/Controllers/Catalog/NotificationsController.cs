using TD.WebApi.Application.Catalog.Notifications;

namespace TD.WebApi.Host.Controllers.Catalog;

public class NotificationsController : VersionedApiController
{
    [HttpPost("search")]
    [OpenApiOperation("Danh sách Notification.", "")]
    public Task<PaginationResponse<NotificationDto>> SearchAsync(SearchNotificationsRequest request)
    {
        return Mediator.Send(request);
    }


    [HttpPost("buoihocbatdau")]
    [OpenApiOperation("Search HardwareDevices using available filters.", "")]
    public Task<string> RecurringNotificationMinuteRequest(RecurringNotificationMinuteRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Chi tiết Notification.", "")]
    public Task<Result<NotificationDetailsDto>> GetAsync(Guid id)
    {
        return Mediator.Send(new GetNotificationRequest(id));
    }

    [HttpPost("send")]
    [OpenApiOperation("Tạo mới Notification.", "")]
    public Task<string> SendNotificationAsync(SendNotificationRequest request)
    {
        return Mediator.Send(request);
    }
    [HttpPost]
    [OpenApiOperation("Tạo mới Notification.", "")]
    public Task<Result<Guid>> CreateAsync(CreateNotificationRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [OpenApiOperation("Cập nhật Notification.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateNotificationRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Xóa Notification.", "")]
    public Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteNotificationRequest(id));
    }

}