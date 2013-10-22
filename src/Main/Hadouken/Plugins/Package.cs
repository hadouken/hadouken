using System;
using System.IO;
using System.Linq;
using Hadouken.Plugins.Metadata;
using Ionic.Zip;

namespace Hadouken.Plugins
{
    public sealed class Package : IPackage
    {
        private readonly IManifest _manifest;

        private Package(IManifest manifest)
        {
            _manifest = manifest;
        }

        public IManifest Manifest
        {
            get { return _manifest; }
        }

        public void Unpack(Stream stream, string outputPath)
        {
            using (var zip = ZipFile.Read(stream))
            {
                zip.ExtractAll(outputPath);
            }
        }

        public static bool TryParse(Stream stream, out IPackage package)
        {
            package = null;

            using (var zip = ZipFile.Read(stream))
            {
                // Read manifest
                var manifestEntry = zip.Entries.SingleOrDefault(e => e.FileName == "manifest.json");

                if (manifestEntry == null)
                    return false;

                using (var memoryStream = new MemoryStream())
                {
                    manifestEntry.Extract(memoryStream);
                    memoryStream.Position = 0;

                    IManifest manifest;

                    if (!Metadata.Manifest.TryParse(memoryStream, out manifest))
                        return false;

                    package = new Package(manifest);
                    return true;
                }
            }
        }
    }
}
