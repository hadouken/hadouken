using Autofac;
using Hadouken.Common;
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
                var service = container.Resolve<IService>();
                var environment = container.Resolve<IEnvironment>();

                if (environment.IsUserInteractive())
                {
                    new ConsoleHost(service).Run(args);
                }
            }
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            
            // Core
            builder.RegisterModule(new CoreModule());

            // Common
            builder.RegisterType<HadoukenEnvironment>().As<IEnvironment>().SingleInstance();

            return builder.Build();
        }
    }
}
