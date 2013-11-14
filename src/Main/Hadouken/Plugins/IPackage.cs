using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hadouken.Framework.IO;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins
{
    public interface IPackage
    {
        IManifest Manifest { get; }

        IFile[] Files { get; }
    }
}
