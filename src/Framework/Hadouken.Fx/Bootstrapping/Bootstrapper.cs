namespace Hadouken.Fx.Bootstrapping
{
    public abstract class Bootstrapper<TContainer> : IBootstrapper
    {
        protected PluginConfiguration Configuration { get; private set; }

        protected TContainer ApplicationContainer { get; private set; }

        public void Initialize(PluginConfiguration configuration)
        {
            Configuration = configuration;
            ApplicationContainer = CreateContainer();
            RegisterComponents(ApplicationContainer);
        }

        public IPluginHost GetHost()
        {
            return GetHost(ApplicationContainer);
        }

        protected abstract TContainer CreateContainer();

        protected abstract void RegisterComponents(TContainer container);

        protected abstract IPluginHost GetHost(TContainer container);
    }
}
