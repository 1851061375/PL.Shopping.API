using TD.WebApi.Application.Catalog.Notifications;
using TD.WebApi.Application.Common.Interfaces;

namespace TD.WebApi.Host.Controllers.Public;

public class NotificationsController : VersionedPulbicApiController
{

    private readonly ICurrentUser _currentUser;

    public NotificationsController(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpPost("search")]
    [Authorize]
    [OpenApiOperation("Danh sách thông báo của cá nhân.", "")]
    public Task<PaginationResponse<NotificationDto>> SearchAsync(SearchNotificationsRequest request)
    {
        request.Topic = _currentUser.GetUserId().ToString();
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [OpenApiOperation("Chi tiết thông báo", "")]
    public Task<Result<NotificationDetailsDto>> GetAsync(Guid id)
    {
        return Mediator.Send(new GetNotificationRequest(id));
    }
    [HttpPut("{id:guid}")]
    [Authorize]
    [OpenApiOperation("Cập nhật trạng thái thông báo.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateNotificationRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [OpenApiOperation("Xóa thông báo.", "")]
    public Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteNotificationRequest(id));
    }
}