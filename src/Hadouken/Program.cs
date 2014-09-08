using System;
using System.ServiceProcess;
using Autofac;
using Hadouken.Common;
using Hadouken.Common.Data;
using Hadouken.Common.DI;
using Hadouken.Common.Logging;
using Hadouken.Core;
using Hadouken.Core.DI;
using Hadouken.Hosts;

namespace Hadouken
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var container = BuildContainer())
            {
                var environment = container.Resolve<IEnvironment>();
                var logger = container.Resolve<ILogger<Program>>();

                // Log unhandled exceptions
                AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs)
                    => logger.Fatal(
                        eventArgs.ExceptionObject as Exception,
                        "Unhandled exception. Hadouken crashing."); 

                // Load extensions
                container.LoadExtensions(environment.GetApplicationRoot());

                // Run migrations
                var migrator = container.Resolve<IMigrator>();
                migrator.Migrate();

                var service = container.Resolve<IHadoukenService>();

                if (environment.IsUserInteractive())
                {
                    new ConsoleHost(service).Run(args);
                }
                else
                {
                    ServiceBase.Run(new HadoukenServiceHost(service));
                }
            }
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule<CoreModule>();

            return builder.Build();
        }
    }
}
