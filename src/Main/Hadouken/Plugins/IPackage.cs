using Hadouken.Framework.IO;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins
{
    public interface IPackage
    {
        IManifest Manifest { get; }

        IFile[] Files { get; }

        byte[] Data { get; }

        IFile GetFile(string path);
    }
}
