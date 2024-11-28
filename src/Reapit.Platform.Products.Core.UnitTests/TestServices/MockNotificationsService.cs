using Reapit.Platform.Products.Core.Services.Notifications;
using Reapit.Platform.Products.Core.Services.Notifications.Models;

namespace Reapit.Platform.Products.Core.UnitTests.TestServices;

public class MockNotificationsService : INotificationsService
{
    public MessageEnvelope? LastMessage { get; private set; }
    
    public async Task<string?> PublishNotificationAsync(MessageEnvelope message, CancellationToken cancellationToken)
    {
        LastMessage = message;
        return await Task.FromResult<string?>(Guid.NewGuid().ToString());
    }
}