using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ECS;
using Constructs;
using System.Collections.Generic;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.EFS;
using Amazon.CDK.AWS.Logs;

namespace Valheim
{
    public class MainStack : Stack
    {
        public class MainStackProps : StackProps
        {
            public bool IsOn { get; set; } = true;
            public string World { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
        }
        internal MainStack(Construct scope, string id, MainStackProps props = null) : base(scope, id, props)
        {
            var world = new ValheimWorld(this, "ValheimWorld", new ValheimWorldProps 
            {
                IsOn = props.IsOn,
                World = props.World,
                Name = props.Name,
                Password = props.Password
            });
        }
    }
}
