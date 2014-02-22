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
        private readonly IManifest _manifest;
        private readonly IFile[] _files;

        internal Package(IManifest manifest, IFile[] files)
        {
            _manifest = manifest;
            _files = files;
        }

        public IManifest Manifest
        {
            get { return _manifest; }
        }

        public IFile[] Files
        {
            get { return _files; }
        }

        public byte[] Data { get; set; }

        public Uri BaseUri { get; set; }

        public IFile GetFile(string path)
        {
            var requestedUri = new Uri(path, UriKind.Relative);

            if (BaseUri == null)
            {
                // Probably an in-memory file.

                return (from file in Files
                    let root = new Uri(file.FullPath, UriKind.Relative)
                    where requestedUri == root
                    select file).FirstOrDefault();
            }

            var f = (from file in Files
                let root = new Uri(file.FullPath, UriKind.Absolute)
                where BaseUri.MakeRelativeUri(root) == requestedUri
                select file).FirstOrDefault();

            return f;
        }

        public static bool TryParse(IDirectory directory, out IPackage package)
        {
            package = null;
            Package p;

            if (!TryParse(directory.Files, out p))
            {
                return false;
            }

            p.BaseUri = new Uri(directory.FullPath, UriKind.Absolute);
            package = p;
            return true;
        }

        public static bool TryParse(IFile file, out IPackage package)
        {
            package = null;
            var data = file.ReadAllBytes();

            using (var ms = new MemoryStream(data))
            {
                Package p;

                if (!TryParse(ms, out p)) return false;

                p.Data = data;
                package = p;

                return true;
            }
        }

        private static bool TryParse(IFile[] files, out Package package)
        {
            package = null;

            if (files == null)
            {
                return false;
            }

            if (!files.Any())
            {
                return false;
            }

            var manifestFile = files.SingleOrDefault(f => f.Name == Metadata.Manifest.FileName);

            if (manifestFile == null)
            {
                return false;
            }

            IManifest manifest;

            using (var stream = manifestFile.OpenRead())
            {
                if (!Metadata.Manifest.TryParse(stream, out manifest))
                {
                    return false;
                }
            }

            // We have a valid manifest. Create package.
            package = new Package(manifest, files);
            return true;
        }

        private static bool TryParse(Stream stream, out Package package)
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
                            var streamFactory = new Func<MemoryStream>(() => new MemoryStream(data));

                            files.Add(new InMemoryFile(streamFactory)
                            {
                                Name = entry.FileName,
                                FullPath = entry.FileName
                            });
                        }
                    }

                    Package p;

                    if (!TryParse(files.ToArray(), out p))
                    {
                        return false;
                    }

                    package = p;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
