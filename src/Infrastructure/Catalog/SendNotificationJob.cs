using FirebaseAdmin.Messaging;
using Hangfire;
using Hangfire.Console.Extensions;
using Hangfire.Server;
using MediatR;
using Microsoft.Extensions.Logging;
using TD.WebApi.Application.Catalog.AppConfigs;
using TD.WebApi.Application.Catalog.Notifications;
using TD.WebApi.Application.Common.Interfaces;
using TD.WebApi.Application.Common.Persistence;
using TD.WebApi.Shared.Notifications;

namespace TD.WebApi.Infrastructure.Catalog;

public class SendNotificationJob : ISendNotificationJob
{
    private readonly ILogger<SendNotificationJob> _logger;
    private readonly ISender _mediator;
    private readonly IProgressBarFactory _progressBar;
    private readonly PerformingContext _performingContext;
    private readonly INotificationSender _notifications;
    private readonly IDapperRepository _dapperRepository;

    public SendNotificationJob(
        ILogger<SendNotificationJob> logger,
        IDapperRepository dapperRepository,
        ISender mediator,
        IProgressBarFactory progressBar,
        PerformingContext performingContext,
        INotificationSender notifications)
    {
        _dapperRepository = dapperRepository;
        _logger = logger;
        _mediator = mediator;
        _progressBar = progressBar;
        _performingContext = performingContext;
        _notifications = notifications;
    }

    private async Task NotifyAsync(string message, int progress, CancellationToken cancellationToken)
    {
        await _notifications.SendToUserAsync(
            new JobNotification()
            {
                JobId = _performingContext.BackgroundJob.Id,
                Message = message,
                Progress = progress
            },
            string.Empty,
            cancellationToken);
    }

    [Queue("notdefault")]
    public async Task SendNotificationAsync(SendNotificationRequest request, CancellationToken cancellationToken)
    {
        await NotifyAsync("FetchProductAsync processing has started", 0, cancellationToken);
        foreach (string topic in request.Topics)
        {
            await Handle(topic, request, null, cancellationToken);
        }

        await NotifyAsync("FetchProductAsync successfully completed", 0, cancellationToken);
    }

    public class DataNotification
    {
        public string? NoiDung { get; set; }
        public string? ImageUrl { get; set; }
    }

    public async Task Handle(string topic, SendNotificationRequest request, Guid? id, CancellationToken cancellationToken)
    {
        try
        {
            var message = new Message()
            {
                Topic = topic,
                Data = new Dictionary<string, string>()
                {
                    ["Data"] = request?.Data ?? string.Empty,
                    ["Id"] = id.ToString() ?? string.Empty,
                    ["ImageUrl"] = string.Empty,
                },
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = request.Title,
                    Body = request.Content,
                    ImageUrl = null
                },
                Android = new FirebaseAdmin.Messaging.AndroidConfig
                {
                    Notification = new FirebaseAdmin.Messaging.AndroidNotification
                    {
                        ChannelId = "CodeMath"
                    },
                    Priority = Priority.High

                },
                Apns = new FirebaseAdmin.Messaging.ApnsConfig
                {
                    Headers = new Dictionary<string, string>()
                    {
                        ["apns-priority"] = "5",
                    },
                    Aps = new FirebaseAdmin.Messaging.Aps
                    {
                        Sound = "default",
                        ThreadId = "CodeMath",
                        ContentAvailable = true
                    }
                }
            };
            var messaging = FirebaseMessaging.DefaultInstance;
            string? result = await messaging.SendAsync(message, false, cancellationToken);


            var tmp = await _mediator.Send(
                    new CreateNotificationRequest
                    {
                        Topic = topic,
                        Title = request.Title,
                        Content = request.Content,
                        Description = request.Description,
                        Link = request.Link,
                        DeepLink = request.DeepLink,
                        CommentId = request.CommentId,
                        DiscussionId = request.DiscussionId,
                        Code = null,
                        Data = request.Data,
                        IsRead = false,
                    },
                    cancellationToken);
        }
        catch
        {

        }
    }



    private async Task<string> GetValueAppConfigs(string key)
    {
        var item = await _dapperRepository.QueryFirstOrDefaultObjectAsync<AppConfigDto>($"SELECT TOP (1) *  FROM [Catalog].[AppConfigs] WHERE [Key] = '{key}' AND DeletedOn IS NULL");
        if (item != null && !string.IsNullOrEmpty(item.Value))
        {
            return item.Value;
        }

        return string.Empty;
    }
}