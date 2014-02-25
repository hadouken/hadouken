using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.Fx.IO;
using Hadouken.Plugins.Metadata;
using Ionic.Zip;

namespace Hadouken.Plugins
{
    public sealed class Package : IPackage
    {
        private readonly IManifest _manifest;
        private readonly IReadOnlyCollection<IFile> _files;

        internal Package(IManifest manifest, IReadOnlyCollection<IFile> files)
        {
            _manifest = manifest;
            _files = files;
        }

        public IManifest Manifest
        {
            get { return _manifest; }
        }

        public IReadOnlyCollection<IFile> Files
        {
            get { return _files; }
        }

        public static bool TryParse(Stream stream, out IPackage package)
        {
            package = null;

            try
            {
                using (var zip = ZipFile.Read(stream))
                {
                    var files = new List<IFile>();

                    foreach (var entry in zip.Entries)
                    {
                        using (var memStream = new MemoryStream())
                        {
                            entry.Extract(memStream);
                            var data = memStream.ToArray();

                            files.Add(new InMemoryFile(entry.FileName, data));
                        }
                    }

                    var manifestFile = files.SingleOrDefault(f => f.FullPath == Metadata.Manifest.FileName);
                    if (manifestFile == null)
                    {
                        return false;
                    }

                    using (var manifestStream = manifestFile.OpenRead())
                    {
                        IManifest manifest;
                        if (!Metadata.Manifest.TryParse(manifestStream, out manifest))
                        {
                            return false;
                        }

                        package = new Package(manifest, files);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
