using Amazon.CDK;
using Constructs;
using System;
using System.Collections.Generic;
using System.Linq;
using static Valheim.MainStack;

namespace Valheim
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            var accountId = app.Node.TryGetContext("AccountId") as string;
            var world = app.Node.TryGetContext("World") as string;
            var name = app.Node.TryGetContext("Name") as string;
            var password = app.Node.TryGetContext("Password") as string;

            new MainStack(app, "ValheimStack", new MainStackProps 
            {
                Env = new Amazon.CDK.Environment
                {
                    Account = accountId,
                    Region = "us-west-2"
                }, 
                World = world,
                Name = name,
                Password = password,
                Tags = new Dictionary<string, string>
                {
                    { "Billing", "smith.colin00@gmail.com" }
                }
            });
            app.Synth();
        }
    }
}
