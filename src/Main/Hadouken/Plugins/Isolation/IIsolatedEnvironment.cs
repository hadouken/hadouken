namespace Hadouken.Plugins.Isolation
{
    public interface IIsolatedEnvironment
    {
        void Load();

        void Unload();

        long GetMemoryUsage();
    }
}
