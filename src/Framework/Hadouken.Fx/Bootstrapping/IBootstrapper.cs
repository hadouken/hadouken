namespace Hadouken.Fx.Bootstrapping
{
    public interface IBootstrapper
    {
        void Initialize(PluginConfiguration configuration);

        IPluginHost GetHost();
    }
}
