using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ECS;
using Constructs;
using System.Collections.Generic;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;

namespace Cdk
{
    public class MainStack : Stack
    {
        internal MainStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var vpc = new Vpc(this, "ValheimVPC");

            var cluster = new Cluster(this, "ValheimCluster", new ClusterProps
            {
                Vpc = vpc
            });

            var taskDefinition = new FargateTaskDefinition(this, "ValheimTask", new FargateTaskDefinitionProps
            {
                Cpu = 256,
                MemoryLimitMiB = 512
            });

            taskDefinition.AddContainer("ValheimContainer", new ContainerDefinitionOptions
            {
                Image = ContainerImage.FromRegistry("lloesche/valheim-server"),
                Environment = new Dictionary<string, string>
                {
                    { "SERVER_NAME", "My Valheim Server" },
                    { "WORLD_NAME", "MyWorld" },
                    { "SERVER_PASS", "secret" }
                },
                PortMappings = new PortMapping[]
                {
                    new PortMapping
                    {
                        ContainerPort = 2456,
                        HostPort = 2456,
                        Protocol = Amazon.CDK.AWS.ECS.Protocol.UDP
                    },
                    new PortMapping
                    {
                        ContainerPort = 2457,
                        HostPort = 2457,
                        Protocol = Amazon.CDK.AWS.ECS.Protocol.UDP
                    },
                    new PortMapping
                    {
                        ContainerPort = 2458,
                        HostPort = 2458,
                        Protocol = Amazon.CDK.AWS.ECS.Protocol.UDP
                    }
                }
            });

            new ApplicationLoadBalancedFargateService(this, "ValheimService", new ApplicationLoadBalancedFargateServiceProps
            {
                Cluster = cluster,
                TaskDefinition = taskDefinition,
                PublicLoadBalancer = true
            });
        }
    }
}
