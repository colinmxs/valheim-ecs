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
            // Create a VPC to deploy the container to
            var vpc = new Vpc(this, "ValheimVpc", new VpcProps
            {
                MaxAzs = 2,
                NatGateways = 1
            });

            // Create a task definition with some basic settings
            var taskDefinition = new TaskDefinition(this, "ValheimTask", new TaskDefinitionProps
            {
                NetworkMode = NetworkMode.AWS_VPC,
                Family = "valheim",
                Compatibility = Compatibility.FARGATE,
                Cpu = "2048",
                MemoryMiB = "4096"
            });

            // Define the Valheim container with the appropriate image and mappings
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

            // Create a cluster where we can deploy the container
            var cluster = new Cluster(this, "ValheimCluster", new ClusterProps
            {
                Vpc = vpc
            });

            // Create a service with our task definition and cluster
            var service = new FargateService(this, "ValheimService", new FargateServiceProps
            {
                Cluster = cluster,
                TaskDefinition = taskDefinition
            });

            // Create a load balancer for the service to listen on
            var loadBalancer = new ApplicationLoadBalancer(this, "ValheimLoadBalancer", new Amazon.CDK.AWS.ElasticLoadBalancingV2.ApplicationLoadBalancerProps
            {
                Vpc = vpc,
                InternetFacing = true
            });

            // Allow traffic to the load balancer on the appropriate ports
            loadBalancer.Connections.AllowFromAnyIpv4(Port.Tcp(80), "Allow HTTP Traffic");
            loadBalancer.Connections.AllowFromAnyIpv4(Port.Tcp(443), "Allow HTTPS Traffic");

            // Create a target group for the service to send traffic to
            var targetGroup = new ApplicationTargetGroup(this, "ValheimTargetGroup", new ApplicationTargetGroupProps
            {
                Vpc = vpc,
                Port = 80,
                Targets = new[] { service },
                HealthCheck = new Amazon.CDK.AWS.ElasticLoadBalancingV2.HealthCheck
                {
                    Interval = Duration.Seconds(30),
                    Path = "/",
                    Protocol = Amazon.CDK.AWS.ElasticLoadBalancingV2.Protocol.HTTP                    
                }
            });

            // Create a listener for the load balancer to listen on
            var listener = loadBalancer.AddListener("ValheimListener", new BaseApplicationListenerProps
            {
                Protocol = ApplicationProtocol.HTTP,
                Port = 80,
                DefaultTargetGroups = new[] { targetGroup }
            });
        }
    }
}
