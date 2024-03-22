using System.ComponentModel;

namespace TD.WebApi.Application.Catalog.Notifications;

public interface ISendNotificationJob : IScopedService
{
    [DisplayName("Send Notification")]
    Task SendNotificationAsync(SendNotificationRequest request, CancellationToken cancellationToken);

}