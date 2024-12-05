namespace ProductsCdk.Constructs;

/// <summary>Properties associated with SNS topic creation.</summary>
/// <param name="TopicName">The name of the topic.</param>
/// <param name="QueueName">The name of the internal queue attached to the topic.</param>
public record SnsTopicAndQueueProps(
    string TopicName,
    string QueueName);