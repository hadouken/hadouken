using System.IO;
using Hadouken.Common;
using Hadouken.Common.IO;
using Ragnar;

namespace Hadouken.Core.BitTorrent {
    public class TorrentInfoRepository : ITorrentInfoRepository {
        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;

        public TorrentInfoRepository(IEnvironment environment,
            IFileSystem fileSystem) {
            this._environment = environment;
            this._fileSystem = fileSystem;
        }

        public void Save(TorrentInfo info) {
            var torrentsPath = this._environment.GetApplicationDataPath().Combine("Torrents");

            using (var creator = new TorrentCreator(info)) {
                var data = creator.Generate();
                var filePath = torrentsPath.CombineWithFilePath(string.Format("{0}.torrent", info.InfoHash));
                var file = this._fileSystem.GetFile(filePath);

                using (var fileStream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None)) {
                    fileStream.Write(data, 0, data.Length);
                }
            }
        }

        public void Remove(string infoHash) {
            var torrentsPath = this._environment.GetApplicationDataPath().Combine("Torrents");
            var torrentsFile = this._fileSystem.GetFile(torrentsPath.CombineWithFilePath(infoHash + ".torrent"));
            var resumeFile = this._fileSystem.GetFile(torrentsPath.CombineWithFilePath(infoHash + ".resume"));

            if (torrentsFile.Exists) {
                torrentsFile.Delete();
            }
            if (resumeFile.Exists) {
                resumeFile.Delete();
            }
        }
    }
}