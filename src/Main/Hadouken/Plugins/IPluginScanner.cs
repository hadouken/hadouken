using System.Collections.Generic;

namespace Hadouken.Plugins
{
    public interface IPluginScanner
    {
        IEnumerable<IPluginManager> Scan();
    }
}
