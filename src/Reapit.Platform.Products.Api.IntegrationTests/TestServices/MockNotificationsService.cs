using Reapit.Platform.Products.Core.Services.Notifications;
using Reapit.Platform.Products.Core.Services.Notifications.Models;

namespace Reapit.Platform.Products.Api.IntegrationTests.TestServices;

/// <summary>Mock implementation of <see cref="INotificationsService"/>.</summary>
public class MockNotificationsService : INotificationsService
{
    public Task<string?> PublishNotificationAsync(MessageEnvelope message, CancellationToken cancellationToken) 
        => Task.FromResult<string?>(Guid.NewGuid().ToString("D"));
}