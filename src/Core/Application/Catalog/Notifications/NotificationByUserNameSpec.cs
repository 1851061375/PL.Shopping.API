namespace TD.WebApi.Application.Catalog.Notifications;
public class NotificationByUserNameSpec : Specification<Notification>
{
    public NotificationByUserNameSpec(string userName, bool? isRead) =>
        Query.Where(p => p.Topic == userName).Where(p => p.IsRead == isRead, isRead.HasValue);
}
