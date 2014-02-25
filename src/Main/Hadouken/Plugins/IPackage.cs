using System.Collections.Generic;
using Hadouken.Framework.IO;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins
{
    public interface IPackage
    {
        IManifest Manifest { get; }

        IReadOnlyCollection<IFile> Files { get; }
    }
}
