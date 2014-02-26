using Hadouken.Fx.IO;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins
{
    public interface IPluginManagerFactory
    {
        IPluginManager Create(IDirectory directory, IManifest manifest);
    }
}
