namespace Hadouken.Fx.Bootstrapping
{
    public interface IBootstrapper
    {
        void Initialize();

        IPluginHost GetHost();
    }
}
