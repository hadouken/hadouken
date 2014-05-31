using Hadouken.Fx.IO;
using NuGet;

namespace Hadouken.Plugins
{
    public interface IPluginManagerFactory
    {
        IPluginManager Create(IDirectory directory, IPackage package);
    }
}
