using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Valheim
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            var accountParam = new Amazon.CDK.CfnParameter(app, "Account", new Amazon.CDK.CfnParameterProps
            {
                Type = "String",
                Description = "AWS Account ID"
            });

            new MainStack(app, "ValheimStack", new StackProps 
            {
                Env = new Amazon.CDK.Environment
                {
                    Account = accountParam.ValueAsString,
                    Region = "us-west-2"
                }
            });
            app.Synth();
        }
    }
}
