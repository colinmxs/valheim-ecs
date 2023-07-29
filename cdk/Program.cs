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
            new MainStack(app, "ValheimStack", new StackProps { });
            app.Synth();
        }
    }
}
