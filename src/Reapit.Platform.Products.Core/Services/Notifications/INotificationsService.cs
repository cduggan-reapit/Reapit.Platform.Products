using Reapit.Platform.Products.Core.Services.Notifications.Models;

namespace Reapit.Platform.Products.Core.Services.Notifications;

/// <summary>Service responsible for publishing notifications to the Products SNS topic.</summary>
public interface INotificationsService
{
    /// <summary>Publish a notification to the SNS topic.</summary>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The message Id when the message is published; otherwise null.</returns>
    Task<string?> PublishNotificationAsync(MessageEnvelope message, CancellationToken cancellationToken);
}