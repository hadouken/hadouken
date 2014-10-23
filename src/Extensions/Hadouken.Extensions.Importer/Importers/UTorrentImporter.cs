using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility;
using Hadouken.Common.IO;
using Hadouken.Common.Messaging;
using Hadouken.Common.Text.BEncoding;

namespace Hadouken.Extensions.Importer.Importers
{
    [Component]
    public sealed class UTorrentImporter : IImporter
    {
        private readonly IFileSystem _fileSystem;
        private readonly IMessageBus _messageBus;

        public UTorrentImporter(IFileSystem fileSystem,
            IMessageBus messageBus)
        {
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");
            if (messageBus == null) throw new ArgumentNullException("messageBus");
            _fileSystem = fileSystem;
            _messageBus = messageBus;
        }

        public string Name
        {
            get { return "uTorrent"; }
        }

        public void Import(string dataPath)
        {
            var dataDirectory = _fileSystem.GetDirectory(dataPath);
            var resumeFile = _fileSystem.GetFile(dataDirectory.Path.CombineWithFilePath("resume.dat"));

            if (!resumeFile.Exists)
            {
                return;
            }

            using(var stream = resumeFile.OpenRead())
            {
                var resumeData = (BEncodedDictionary) new BDecoder().Decode(stream);

                foreach (var item in resumeData)
                {
                    if (item.Key == ".fileguard") continue;

                    // Read torrent file
                    var torrentFile = _fileSystem.GetFile(dataDirectory.Path.CombineWithFilePath((string)item.Key));
                    if (!torrentFile.Exists) continue;

                    var torrentItems = (BEncodedDictionary) item.Value;
                    var savePath = (string) (BEncodedString) torrentItems["path"];

                    // uTorrent stores its save paths with the torrent name appended,
                    // so we need to remove the last segment.
                    savePath = savePath.Substring(0, savePath.LastIndexOf("\\", StringComparison.Ordinal));

                    // Add it
                    using (var torrentFileStream = torrentFile.OpenRead())
                    {
                        var data = new byte[torrentFileStream.Length];
                        torrentFileStream.Read(data, 0, data.Length);

                        _messageBus.Publish(new AddTorrentMessage(data) {SavePath = savePath});
                    }
                }
            }
        }
    }
}
