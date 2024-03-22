namespace TD.WebApi.Application.Catalog.Notifications;

public class RecurringNotificationMinuteRequest : IRequest<string>
{
    public string JobName { get; set; }
    public string CronExpression { get; set; }

    public RecurringNotificationMinuteRequest(string jobName, string cronExpression)
    {
        JobName = jobName;
        CronExpression = cronExpression;
    }
}

