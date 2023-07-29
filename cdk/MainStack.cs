using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ECS;
using Constructs;
using System.Collections.Generic;

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

            var taskDefinition = new FargateTaskDefinition(this, "ValheimTaskDefinition", new FargateTaskDefinitionProps
            {
                Cpu = 256,
                MemoryLimitMiB = 512
            });

            taskDefinition.AddContainer("ValheimContainer", new ContainerDefinitionOptions
            {
                Image = ContainerImage.FromRegistry("lloesche/valheim-server"),
                Environment = new Dictionary<string, string>
                {
                    { "SERVER_NAME", (string)scope.Node.TryGetContext("valheim-server/server-name") },
                    { "WORLD_NAME", (string)scope.Node.TryGetContext("valheim-server/world-name") },
                    { "SERVER_PASS", (string)scope.Node.TryGetContext("valheim-server/server-password") }
                }
            });

            var securityGroup = SecurityGroup.FromSecurityGroupId(this, "ValheimSecurityGroup", cluster.Connections.SecurityGroups[0].SecurityGroupId);

            securityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Udp(2456), "Allow Valheim traffic");
            securityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Udp(2457), "Allow Valheim traffic");
            securityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Udp(2458), "Allow Valheim traffic");

            new ApplicationLoadBalancedFargateService(this, "ValheimService", new ApplicationLoadBalancedFargateServiceProps
            {
                Cluster = cluster,
                TaskDefinition = taskDefinition,
                PublicLoadBalancer = true,
                AssignPublicIp = true,
                DesiredCount = 1
            });
        }
    }
}
