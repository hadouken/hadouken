namespace Hadouken.Fx
{
    public interface IBootstrapper
    {
        void Initialize();

        IPluginHost GetHost();
    }
}
