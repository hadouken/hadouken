using Autofac;
using Hadouken.Logging;
using Serilog;

namespace Hadouken.DI.Modules
{
    public sealed class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InMemorySink>().As<IInMemorySink>().SingleInstance();
            builder.Register(c =>
            {
                var memSink = c.Resolve<IInMemorySink>();
                return new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Sink(memSink)
                    .WriteTo.ColoredConsole()
                    .CreateLogger();
            });
        }
    }
}
