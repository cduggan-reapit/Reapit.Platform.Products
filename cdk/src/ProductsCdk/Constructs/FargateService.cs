using System;
using System.Collections.Generic;
using Amazon.CDK.AWS.Apigatewayv2;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.ServiceDiscovery;
using Amazon.CDK.AwsApigatewayv2Integrations;
using Constructs;

namespace ProductsCdk.Constructs;

public class FargateService : Construct
{
    /// <summary>The security group associated with the ECS cluster.</summary>
    public EC2.SecurityGroup SecurityGroup { get; }
    
    /// <summary>Initializes a new instance of the <see cref="FargateService"/> class.</summary>
    /// <param name="scope">The scope in which to define this construct.</param>
    /// <param name="id">The scoped construct ID</param>
    /// <param name="props">The construct properties.</param>
    public FargateService(Construct scope, string id, FargateServiceProps props) 
        : base(scope, id)
    {
        var taskRole = new IAM.Role(this, "role", new IAM.RoleProps
        {
            RoleName = $"{id}-service-role",
            AssumedBy = new IAM.ServicePrincipal("ecs-tasks.amazonaws.com", new IAM.ServicePrincipalOpts()),
            Description = $"ECS task execution role for {id}",
            InlinePolicies = props.InlinePolicies, 
            ManagedPolicies = props.ManagedPolicies 
        });
        
        SecurityGroup = new EC2.SecurityGroup(this, "security-group", new EC2.SecurityGroupProps
        {
            Vpc = props.Vpc,
            SecurityGroupName = $"{id} - ECS Task Security Group",
            Description = $"{id} ECS task security group.",
            AllowAllOutbound = true
        });
        
        var imageRepository = new ECR.Repository(this, "repository", new ECR.RepositoryProps
        {
            RepositoryName = $"{id}-repository",
            EmptyOnDelete = true,
            RemovalPolicy = RemovalPolicy.DESTROY
        });
        
        var logGroup = new LogGroup(this, "logs", new LogGroupProps
        {
            LogGroupName = props.Context.Api.LogGroupName,
            Retention = RetentionDays.ONE_MONTH,
            RemovalPolicy = RemovalPolicy.DESTROY
        });
        
        var vpcLink = new VpcLink(this, "vpc-link", new VpcLinkProps
        {
            Vpc = props.Vpc,
            VpcLinkName = $"{props.Context.Api.Name} API VPC Link",
            Subnets = new EC2.SubnetSelection
            {
                SubnetGroupName = "Private"
            }
        });

        var dnsNamespace = new PrivateDnsNamespace(this, "namespace", new PrivateDnsNamespaceProps
        {
            Name = props.Context.Api.DnsNamespace,
            Vpc = props.Vpc
        });
        
        var cluster = new ECS.Cluster(this, "cluster", new ECS.ClusterProps
        {
            Vpc = props.Vpc,
            ClusterName = $"{props.Context.Api.Name}-cluster",
            EnableFargateCapacityProviders = true
        });
        
        var taskDefinition = new ECS.FargateTaskDefinition(this, "task", new ECS.FargateTaskDefinitionProps
        {
            Cpu = 256,
            MemoryLimitMiB = 512,
            TaskRole = taskRole
        });

        taskDefinition.AddContainer("container", new ECS.ContainerDefinitionOptions
        {
            Image = ECS.ContainerImage.FromEcrRepository(imageRepository, "latest"),
            PortMappings = [new ECS.PortMapping { ContainerPort = 80, Name = "default", Protocol = ECS.Protocol.TCP }],
            HealthCheck = new ECS.HealthCheck
            {
                Command = ["CMD-SHELL", "curl -f http://localhost:80/api/health || exit 1"],
                Retries = 3,
                StartPeriod = Duration.Seconds(30),
                Interval = Duration.Seconds(150),
                Timeout = Duration.Seconds(5)
            },
            Logging = new ECS.AwsLogDriver(new ECS.AwsLogDriverProps
            {
                StreamPrefix = id,
                LogGroup = logGroup
            }), 
            Environment = new Dictionary<string, string>
            {
                { "logGroup", logGroup.LogGroupName },
                { "environment", props.Context.Environment.FullName }
            } 
        });
        
        SecurityGroup.AddIngressRule(EC2.Peer.Ipv4(props.Vpc.VpcCidrBlock), new EC2.Port(new EC2.PortProps { Protocol = EC2.Protocol.TCP, FromPort = 80, ToPort = 80, StringRepresentation = "HTTP" }));

        var service = new ECS.FargateService(this, "service", new ECS.FargateServiceProps
        {
            ServiceName = $"{props.Context.Api.Name}-service",
            Cluster = cluster,
            CapacityProviderStrategies = [
                new ECS.CapacityProviderStrategy { CapacityProvider = "FARGATE_SPOT", Weight = 1 },
                new ECS.CapacityProviderStrategy { CapacityProvider = "FARGATE", Weight = 0 }
            ],
            VpcSubnets = new EC2.SubnetSelection { SubnetGroupName = "Private" },
            SecurityGroups = [SecurityGroup],
            PlatformVersion = ECS.FargatePlatformVersion.VERSION1_4,
            TaskDefinition = taskDefinition,
            CircuitBreaker = new ECS.DeploymentCircuitBreaker {
                Rollback = true
            },
            AssignPublicIp = false,
            DesiredCount = 1,
            CloudMapOptions = new ECS.CloudMapOptions
            {
                Name = "service",
                CloudMapNamespace = dnsNamespace,
                DnsRecordType = DnsRecordType.SRV
            }
        });

        var cloudMapService = service.CloudMapService ?? throw new InvalidOperationException("CloudMap service is null");

        var apiGateway = new HttpApi(this, "gateway", new HttpApiProps
        {
            ApiName = "Platform Product Management API",
            DefaultIntegration = new HttpServiceDiscoveryIntegration("api-discovery", cloudMapService,
                new HttpServiceDiscoveryIntegrationProps
                {
                    VpcLink = vpcLink
                })
        });

        _ = new CfnOutput(this, "url", new CfnOutputProps
        {
            ExportName = props.Context.Api.UrlOutputName,
            Value = apiGateway.Url ?? "Something went wrong"
        });
    }
}