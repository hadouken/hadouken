using System.Threading;
using System.Threading.Tasks;

namespace Hadouken.Plugins
{
    public interface IPluginManager
    {
        IPackage Package { get; }

        PluginState State { get; }

        string ErrorMessage { get; }

        long GetMemoryUsage();

        void Load();

        void Unload();
    }

    public static class PluginManagerExtensions
    {
        public static void WaitForState(this IPluginManager pluginManager, PluginState state)
        {
            while (pluginManager.State != state)
            {
                Thread.Sleep(100);
            }
        }

        public static Task WaitForStateAsync(this IPluginManager pluginManager, PluginState state)
        {
            return Task.Factory.StartNew(() => pluginManager.WaitForState(state));
        }
    }
}
