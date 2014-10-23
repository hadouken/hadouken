using System;
using System.Linq;
using Hadouken.Common;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility;
using Hadouken.Common.IO;
using Hadouken.Common.Messaging;
using Hadouken.Common.Text.BEncoding;

namespace Hadouken.Extensions.Importer.Importers
{
    [Component]
    public sealed class QBittorrentImporter : IImporter
    {
        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly IMessageBus _messageBus;

        public QBittorrentImporter(IEnvironment environment,
            IFileSystem fileSystem,
            IMessageBus messageBus)
        {
            if (environment == null) throw new ArgumentNullException("environment");
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");
            if (messageBus == null) throw new ArgumentNullException("messageBus");
            _environment = environment;
            _fileSystem = fileSystem;
            _messageBus = messageBus;
        }

        public string Name
        {
            get { return "qBittorrent"; }
        }

        public void Import(string dataPath)
        {
            if (string.IsNullOrEmpty(dataPath))
            {
                throw new ArgumentException("Path required.", "dataPath");
            }

            var dataDirectory = _fileSystem.GetDirectory(dataPath);

            if (!dataDirectory.Exists)
            {
                return;
            }

            var targetPath = _environment.GetApplicationDataPath().Combine("Torrents");

            // Iterate torrent files. For each file, also check if we have a fast resume
            // file. If so, copy it to torrents folder and then read save path from it.

            foreach(var torrentFile in dataDirectory.GetFiles("*.torrent", SearchScope.Current))
            {
                var resumePath = torrentFile.Path.ChangeExtension(".fastresume");
                var resumeFile = _fileSystem.GetFile(resumePath);

                string savePath = null;

                if (resumeFile.Exists)
                {
                    var resumeTargetPath = targetPath.CombineWithFilePath(torrentFile.Path.GetFilename().ChangeExtension(".resume"));
                    resumeFile.Copy(resumeTargetPath, false);

                    var resumeTargetFile = _fileSystem.GetFile(resumeTargetPath);

                    // Now read save path
                    using (var stream = resumeTargetFile.OpenRead())
                    {
                        var data = (BEncodedDictionary) new BDecoder().Decode(stream);
                        savePath = (BEncodedString) data["qBt-savePath"];

                        // Replace forward slashes with backward slashes
                        // and remove trailing /'s
                        savePath = savePath.Replace('/', System.IO.Path.DirectorySeparatorChar);
                        savePath = savePath.TrimEnd('/');
                    }
                }

                using (var stream = torrentFile.OpenRead())
                {
                    var data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);

                    _messageBus.Publish(new AddTorrentMessage(data) {SavePath = savePath});
                }
            }
        }
    }
}
