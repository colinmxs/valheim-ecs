using Amazon.CDK;
using Constructs;
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
            var accountId = app.Node.TryGetContext("AccountId") as string;
            new MainStack(app, "ValheimStack", new StackProps 
            {
                Env = new Amazon.CDK.Environment
                {
                    Account = accountId,
                    Region = "us-west-2"
                }
            });
            app.Synth();
        }
    }
}
