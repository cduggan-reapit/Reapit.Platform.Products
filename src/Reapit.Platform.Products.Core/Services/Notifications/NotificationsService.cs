using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reapit.Platform.Cloud.Services.Messaging;
using Reapit.Platform.Products.Core.Configuration;
using Reapit.Platform.Products.Core.Extensions;
using Reapit.Platform.Products.Core.Services.Notifications.Models;

namespace Reapit.Platform.Products.Core.Services.Notifications;

/// <inheritdoc/>
public class NotificationsService : INotificationsService
{
    private readonly IMessagingService _messagingService;
    private readonly IOptions<NotificationTopicConfiguration> _options;
    private readonly ILogger<NotificationsService> _logger;

    /// <summary>Initializes a new instance of the <see cref="NotificationsService"/> class.</summary>
    /// <param name="messagingService">The AWS messaging service.</param>
    /// <param name="options">The configuration accessor.</param>
    /// <param name="logger">The logging service.</param>
    public NotificationsService(
        IMessagingService messagingService,
        IOptions<NotificationTopicConfiguration> options,
        ILogger<NotificationsService> logger)
    {
        _messagingService = messagingService;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<string?> PublishNotificationAsync(MessageEnvelope message, CancellationToken cancellationToken)
    {
        var topicArn = _options.Value.Arn;
        if (string.IsNullOrWhiteSpace(topicArn))
        {
            // Don't try to publish if the SNS topic isn't configured (e.g. local dev)
            _logger.LogWarning("Message not published (no topic configured): {message}", message.ToJson());
            return null;
        }
        
        try
        {
            _logger.LogDebug("Publishing message: {message}", message.ToJson());
            return await _messagingService.PublishMessageToTopicAsync(topicArn, message.ToJson(),
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            // We want to survive this - the change should have been made before the message is sent so returning a non
            // success response would be misleading to consumers.  It's an "us" problem, not a "them" problem.
            _logger.LogError(ex, "Failed to publish to topic: {topic} - {message}", topicArn, ex.Message.ToJson());
            return null;
        }
    }
}