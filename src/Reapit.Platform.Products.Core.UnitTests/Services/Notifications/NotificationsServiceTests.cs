using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute.ExceptionExtensions;
using Reapit.Platform.Cloud.Identity;
using Reapit.Platform.Cloud.Services.Messaging;
using Reapit.Platform.Products.Core.Configuration;
using Reapit.Platform.Products.Core.Services.Notifications;
using Reapit.Platform.Products.Core.Services.Notifications.Models;

namespace Reapit.Platform.Products.Core.UnitTests.Services.Notifications;

public class NotificationsServiceTests
{
    private readonly IMessagingService _messagingService = Substitute.For<IMessagingService>();
    private readonly IOptions<NotificationTopicConfiguration> _options = Substitute.For<IOptions<NotificationTopicConfiguration>>();
    private readonly ILogger<NotificationsService> _logger = Substitute.For<ILogger<NotificationsService>>();
    
    /*
     * PublishNotificationAsync
     */

    [Fact]
    public async Task PublishNotificationAsync_DoesNotPublish_WhenTopicNotConfigured()
    {
        _options.Value.Returns(new NotificationTopicConfiguration { Arn = string.Empty });
        var sut = CreateSut();
        var actual = await sut.PublishNotificationAsync(new MessageEnvelope("entityType", "action", new { }), default);
        actual.Should().BeNull();
        
        _logger.Received(1).Log(LogLevel.Warning, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception?>(), Arg.Any<Func<object, Exception?, string>>());
    }
    
    [Fact]
    public async Task PublishNotificationAsync_LogsError_WhenFailedToPublish()
    {
        const string topicArn = "topicArn";

        _options.Value.Returns(new NotificationTopicConfiguration { Arn = topicArn });
        
        _messagingService.PublishMessageToTopicAsync(topicArn, Arg.Any<string>(), Arg.Any<AmazonClientConfiguration?>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("simulated failure"));
        
        var sut = CreateSut();
        var actual = await sut.PublishNotificationAsync(new MessageEnvelope("entityType", "action", new { }), default);
        actual.Should().BeNull();
        
        _logger.Received(1).Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception?>(), Arg.Any<Func<object, Exception?, string>>());
    }
    
    [Fact]
    public async Task PublishNotificationAsync_ReturnsMessageId_WhenMessagePublished()
    {
        const string topicArn = "topicArn";
        const string expected = "messageId";
        
        _options.Value.Returns(new NotificationTopicConfiguration { Arn = topicArn });
        
        _messagingService.PublishMessageToTopicAsync(topicArn, Arg.Any<string>(), Arg.Any<AmazonClientConfiguration?>(), Arg.Any<CancellationToken>())
            .Returns(expected);
        
        var sut = CreateSut();
        var actual = await sut.PublishNotificationAsync(new MessageEnvelope("entityType", "action", new { }), default);
        actual.Should().Be(expected);
        
        // No warnings or errors (just to check the route is clean...)
        _logger.DidNotReceive().Log(LogLevel.Warning, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception?>(), Arg.Any<Func<object, Exception?, string>>());
        _logger.DidNotReceive().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception?>(), Arg.Any<Func<object, Exception?, string>>());
    }
    
    /*
     * Private methods
     */

    private NotificationsService CreateSut()
        => new(_messagingService, _options, _logger);
}