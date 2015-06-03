using Autofac;
using Hadouken.Common.Data;
using Hadouken.Common.IO;
using Hadouken.Common.Logging;
using Hadouken.Common.Messaging;
using Hadouken.Common.Net;
using Hadouken.Common.Reflection;
using Hadouken.Common.Text;
using Hadouken.Common.Timers;
using Serilog;
using Path = System.IO.Path;

namespace Hadouken.Common.DI {
    public class CommonModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType<JsonSerializer>().As<IJsonSerializer>().SingleInstance();
            builder.RegisterType<HadoukenConsole>().As<IConsole>().SingleInstance();
            builder.RegisterType<HadoukenEnvironment>().As<IEnvironment>().SingleInstance();
            builder.RegisterType<AssemblyNameFinder>().As<IAssemblyNameFinder>().SingleInstance();
            builder.RegisterType<EmbeddedResourceFinder>().As<IEmbeddedResourceFinder>().SingleInstance();
            builder.RegisterType<KeyValueStore>().As<IKeyValueStore>().SingleInstance();

            // Common.Data
            builder.RegisterType<SqlMigrator>().As<IMigrator>().SingleInstance();
            builder.Register<IDbConnection>(c => {
                var env = c.Resolve<IEnvironment>();
                return new DbConnection(env.GetConnectionString("Hadouken"));
            });

            // Common.Logging
            builder.RegisterType<LoggerRepository>()
                .AsSelf()
                .As<ILoggerRepository>()
                .SingleInstance();

            builder.Register(context => {
                var repo = context.Resolve<LoggerRepository>();
                var env = context.Resolve<IEnvironment>();
                var path = env.GetAppSetting("Path:Logs");

                return new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.ColoredConsole()
                    .WriteTo.File(Path.Combine(path, "log.txt"))
                    .WriteTo.Sink(repo)
                    .CreateLogger();
            }).ExternallyOwned()
                .SingleInstance();

            builder.RegisterGeneric(typeof (SerilogLogger<>)).As(typeof (ILogger<>));

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