using System.Collections.Generic;
using ProductsCdk.Models;

namespace ProductsCdk.Constructs;

/// <summary>Properties associated with fargate service creation.</summary>
/// <param name="Context">The CDK context model.</param>
/// <param name="Vpc">The VPC in which the API will be created.</param>
/// <param name="ManagedPolicies">The managed policies to attach to the role that instances of the API will assume.</param>
/// <param name="ManagedPolicies">The inline policies to attach to the role that instances of the API will assume.</param>
public record FargateServiceProps(
    CdkContext Context,
    EC2.IVpc Vpc,
    IAM.IManagedPolicy[]? ManagedPolicies,
    Dictionary<string, IAM.PolicyDocument>? InlinePolicies)
    : IResourceProps;