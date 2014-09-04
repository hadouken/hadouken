using System.Linq;
using Autofac;
using Autofac.Core;
using Hadouken.Common.Data;
using Hadouken.Common.IO;
using Hadouken.Common.Logging;
using Hadouken.Common.Messaging;
using Hadouken.Common.Net;
using Hadouken.Common.Reflection;
using Hadouken.Common.Text;
using Hadouken.Common.Timers;
using Serilog;
using ILogger = Hadouken.Common.Logging.ILogger;

namespace Hadouken.Common.DI
{
    public class CommonModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            // Fix Type injection for ILogger
            registration.Preparing += (sender, args) =>
            {
                var type = args.Component.Activator.LimitType;
                var logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.ColoredConsole()
                    .CreateLogger();

                args.Parameters = args.Parameters.Union(new[]
                {
                    new ResolvedParameter(
                        (p, i) => p.ParameterType == typeof (ILogger),
                        (p, i) => new SerilogLogger(logger, type))
                });
            };
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JsonSerializer>().As<IJsonSerializer>().SingleInstance();
            builder.RegisterType<HadoukenConsole>().As<IConsole>().SingleInstance();
            builder.RegisterType<HadoukenEnvironment>().As<IEnvironment>().SingleInstance();
            builder.RegisterType<AssemblyNameFinder>().As<IAssemblyNameFinder>().SingleInstance();
            builder.RegisterType<KeyValueStore>().As<IKeyValueStore>().SingleInstance();

            // Common.Data
            builder.RegisterType<SqlMigrator>().As<IMigrator>().SingleInstance();
            builder.RegisterType<DbConnection>().As<IDbConnection>().SingleInstance();

            // Common.Logging (when doing Resolve manually)
            var logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.ColoredConsole()
                    .CreateLogger();

            builder.RegisterType<SerilogLogger>().As<ILogger>()
                .WithParameter("logger", logger)
                .WithParameter("source", typeof (void));

            // Common.Net
            builder.RegisterType<HttpClientWrapper>().As<IHttpClient>();
            builder.RegisterType<SmtpClientFactory>().As<ISmtpClientFactory>().SingleInstance();

            // Common.Messaging
            builder.RegisterType<MessageBus>().As<IMessageBus>().SingleInstance();

            // Common.IO
            builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
            builder.RegisterType<Globber>().As<IGlobber>().SingleInstance();

            // Common.Timers
            builder.RegisterType<TimerFactory>().As<ITimerFactory>().SingleInstance();
        }
    }
}
