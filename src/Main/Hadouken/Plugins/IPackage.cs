using System.IO;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins
{
    public interface IPackage
    {
        IManifest Manifest { get; }

        void Unpack(Stream stream, string outputPath);
    }
}
