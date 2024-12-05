using Amazon.CDK.AWS.SNS.Subscriptions;
using Constructs;

namespace ProductsCdk.Constructs;

/// <summary>Construct definition for an SNS topic and attached SQS queue.</summary>
public class SnsTopicAndQueue : Construct
{
    /// <summary>The SNS topic.</summary>
    public SNS.ITopic Topic { get;}
    
    /// <summary>The SQS queue.</summary>
    public SQS.IQueue Queue { get; }
    
    /// <summary>Initializes a new instance of the <see cref="SnsTopicAndQueue"/> class.</summary>
    /// <param name="scope">The scope in which to define this construct.</param>
    /// <param name="id">The scoped construct ID</param>
    /// <param name="props">The construct properties.</param>
    public SnsTopicAndQueue(Construct scope, string id, SnsTopicAndQueueProps props)
        : base(scope, id)
    {
        Topic = new SNS.Topic(this, "topic", new SNS.TopicProps
        {
            DisplayName = props.TopicName,
            TopicName = $"{id}-topic"
        });
        
        Queue = new SQS.Queue(this, "queue", new SQS.QueueProps { QueueName = props.QueueName });
        
        Topic.AddSubscription(new SqsSubscription(Queue));

        _ = new CfnOutput(this, "topic-arn-output", new CfnOutputProps
        {
            ExportName = "Products-NotificationTopicArn",
            Value = Topic.TopicArn,
            Description = "The ARN of the organisations service notifications SNS topic."
        });
    }
}