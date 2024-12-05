using System.Collections.Generic;
using System.Text.Json;
using Constructs;
using ProductsCdk.Constructs;
using ProductsCdk.Models;

namespace ProductsCdk.Stacks;

public class ProductServiceStack : Stack
{
    internal ProductServiceStack(Construct scope, string id, ProductServiceStackProps props) 
        : base(scope, id, props)
    {
        // Create a VPC to plop the service into
        // var vpc = new ServiceVpc(this, $"{id}-vpc", new ServiceVpcProps("Access Management VPC"));
        var vpc = EC2.Vpc.FromLookup(this, "platform-vpc", new EC2.VpcLookupOptions
        {
            Tags = new Dictionary<string, string> { { "platform", "vpc" } }
        });
        
        var topic = new SnsTopicAndQueue(this, $"{id}-notifications", new SnsTopicAndQueueProps("products-notifications", "products-notifications-internal"));
       
        // Create the fargate service
        var fargate = new FargateService(this, $"{id}-api", 
            new FargateServiceProps(
                Context: props.Context, 
                Vpc: vpc, 
                ManagedPolicies: null, 
                InlinePolicies: new Dictionary<string, IAM.PolicyDocument> {
                { "ReadParamStorePolicy", GetReadParamStorePolicy(props.Context) },
                { "ReadSecretsPolicy", GetReadSecretsPolicy(props.Context) },
                { "PublishNotificationsPolicy", GetPublishSnsPolicy(props.Context, topic.Topic) }
            }));
        
        // Allow ECS to access the database
        var databaseSecurityGroupId = Fn.ImportValue("Platform-Database-SecurityGroup-Id");
        var databaseSecurityGroup = EC2.SecurityGroup.FromSecurityGroupId(this, "db-sg", databaseSecurityGroupId);
        databaseSecurityGroup.AddIngressRule(peer: fargate.SecurityGroup,
            connection: new EC2.Port(new EC2.PortProps { Protocol = EC2.Protocol.TCP, FromPort = 3306, ToPort = 3306, StringRepresentation = "Aurora/MySQL"}),
            description: "Allow ECS to access the database");
        
        // Output details to SSM parameter
        var idpParam = Fn.ImportValue("Platform-Auth0-ClientParameter");
        var outputParameterObject = new
        {
            EnvironmentName = props.Context.Environment.FullName,
            NotificationTopic = new
            {
                Arn = topic.Topic.TopicArn,
                Name = topic.Topic.TopicName
            },
            Database = new
            {
                Name = props.Context.Database.Name,
                User = props.Context.Database.Secret
            },
            IdentityProviderClientDetails = idpParam
        };
        
        _ = new SSM.StringParameter(this, $"{id}-configuration", new SSM.StringParameterProps
        {
            ParameterName = props.Context.Parameter,
            DataType = SSM.ParameterDataType.TEXT,
            StringValue = JsonSerializer.Serialize(outputParameterObject)
        });
    }
    
    private static IAM.PolicyDocument GetReadParamStorePolicy(CdkContext context) 
        => new(new IAM.PolicyDocumentProps
        {
            Statements = 
            [
                new IAM.PolicyStatement(new IAM.PolicyStatementProps
                {
                    Sid = "VisualEditor0",
                    Effect = IAM.Effect.ALLOW,
                    Actions = ["ssm:PutParameter", "ssm:GetParameter"],
                    Resources = [$"arn:aws:ssm:{context.Region}:{context.AccountId}:parameter/Platform/*"]
                })
            ]
        });
    
    private static IAM.PolicyDocument GetReadSecretsPolicy(CdkContext context) 
        => new(new IAM.PolicyDocumentProps
        {
            Statements = 
            [
                new IAM.PolicyStatement(new IAM.PolicyStatementProps
                {
                    Sid = "VisualEditor0",
                    Effect = IAM.Effect.ALLOW,
                    Actions = [
                        "secretsmanager:GetResourcePolicy", 
                        "secretsmanager:GetSecretValue",
                        "secretsmanager:DescribeSecret",
                        "secretsmanager:ListSecretVersionIds"
                    ],
                    Resources = [$"arn:aws:secretsmanager:{context.Region}:{context.AccountId}:secret:*"]
                })
            ]
        });
    
    private static IAM.PolicyDocument GetPublishSnsPolicy(CdkContext context, SNS.ITopic topic) 
        => new(new IAM.PolicyDocumentProps
        {
            Statements = 
            [
                new IAM.PolicyStatement(new IAM.PolicyStatementProps
                {
                    Sid = "VisualEditor0",
                    Effect = IAM.Effect.ALLOW,
                    Actions = [
                        "sns:Publish"
                    ],
                    Resources = [$"arn:aws:sns:{context.Region}:{context.AccountId}:{topic.TopicName}"]
                })
            ]
        });
}