using Autofac;
using Hadouken.Framework.Plugins;

namespace Hadouken.Framework
{
    public abstract class Bootstrapper
    {
        private IBootConfig _bootConfig;

        protected IBootConfig Configuration {
            get { return _bootConfig; }
        }

        public virtual IPluginHost Bootstrap(IBootConfig config)
        {
            _bootConfig = config;

            var builder = new ContainerBuilder();
            RegisterFrameworkComponents(builder);
            RegisterDependencies(builder);
            RegisterPlugin(builder);

            var container = builder.Build();
            return container.Resolve<IPluginHost>();
        }

        public abstract void RegisterFrameworkComponents(ContainerBuilder builder);

        public abstract void RegisterPlugin(ContainerBuilder builder);

        public virtual void RegisterDependencies(ContainerBuilder containerBuilder) { }
    }
}
