using Autofac;
using Hadouken.Common;
using Hadouken.Common.Data;
using Hadouken.Common.IO;
using Hadouken.Common.Logging;
using Hadouken.Common.Messaging;
using Hadouken.Common.Net;
using Hadouken.Common.Reflection;
using Hadouken.Common.Text;
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
            }
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            
            // Core
            builder.RegisterModule(new CoreModule());

            // Common
            builder.RegisterType<JsonSerializer>().As<IJsonSerializer>().SingleInstance();
            builder.RegisterType<HadoukenEnvironment>().As<IEnvironment>().SingleInstance();
            builder.RegisterType<AssemblyNameFinder>().As<IAssemblyNameFinder>().SingleInstance();
            builder.RegisterType<KeyValueStore>().As<IKeyValueStore>().SingleInstance();

            // Common.Data
            builder.RegisterType<SqlMigrator>().As<IMigrator>().SingleInstance();
            builder.RegisterType<DbConnection>().As<IDbConnection>().SingleInstance();

            // Common.Net
            builder.RegisterType<HttpClientWrapper>().As<IHttpClient>();
            builder.RegisterType<SmtpClientFactory>().As<ISmtpClientFactory>().SingleInstance();

            // Common.Logging
            builder.RegisterType<ConsoleLogger>().As<ILogger>();
            builder.RegisterType<HadoukenConsole>().As<IConsole>().SingleInstance();

            // Common.Messaging
            builder.RegisterType<MessageBus>().As<IMessageBus>().SingleInstance(); 

            // Common.IO
            builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
            builder.RegisterType<Globber>().As<IGlobber>().SingleInstance();

            return builder.Build();
        }
    }
}
