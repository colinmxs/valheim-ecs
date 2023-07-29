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
        internal MainStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var accountParam = new Amazon.CDK.CfnParameter(this, "Account", new Amazon.CDK.CfnParameterProps
            {
                Type = "String",
                Description = "AWS Account ID"
            });
            
            var world = new ValheimWorld(this, "ValheimWorld", accountParam.ValueAsString);
        }
    }
}
