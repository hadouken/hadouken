using Autofac;

namespace Hadouken.Framework.DI
{
    public class ConfigModule : Module
    {
        private readonly IBootConfig _config;

        public ConfigModule(IBootConfig config)
        {
            _config = config;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(_ => _config);
        }
    }
}
