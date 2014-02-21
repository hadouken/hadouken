using System.Collections.Generic;

namespace Hadouken.Plugins
{
    public interface IPackageFactory
    {
        IEnumerable<IPackage> Scan();
    }
}
