using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.Framework.IO;
using Hadouken.Plugins.Metadata;
using Ionic.Zip;

namespace Hadouken.Plugins
{
    public sealed class Package : IPackage
    {
        private readonly string _sourcePath;
        private readonly IManifest _manifest;
        private readonly IFile[] _files;

        private Package(IManifest manifest, IFile[] files)
            : this(manifest, files, String.Empty)
        {
        }

        private Package(IManifest manifest, IFile[] files, string sourcePath)
        {
            _manifest = manifest;
            _files = files;
            _sourcePath = sourcePath;
        }

        public IManifest Manifest
        {
            get { return _manifest; }
        }

        public IFile[] Files
        {
            get { return _files; }
        }

        public string Path
        {
            get { return _sourcePath; }
        }

        public static bool TryParse(string path, out IPackage package)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return TryParse(path, fileStream, out package);
            }
        }

        public static bool TryParse(Stream stream, out IPackage package)
        {
            return TryParse(null, stream, out package);
        }

        private static bool TryParse(string path, Stream stream, out IPackage package)
        {
            package = null;

            try
            {
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

                        // Valid manifest, lets load all files
                        var files = new List<IFile>();

                        foreach (var entry in zip.Entries)
                        {
                            using (var ms = new MemoryStream())
                            {
                                entry.Extract(ms);

                                var data = ms.ToArray();
                                var streamFactory = new Func<MemoryStream>(() => new MemoryStream(data));

                                files.Add(new InMemoryFile(streamFactory)
                                {
                                    Name = entry.FileName
                                });
                            }
                        }

                        package = new Package(manifest, files.ToArray(), path);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
