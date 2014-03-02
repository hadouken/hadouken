namespace Hadouken.Fx
{
    public abstract class Bootstrapper<TContainer> : IBootstrapper
    {
        protected TContainer ApplicationContainer { get; private set; }

        public void Initialize()
        {
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
