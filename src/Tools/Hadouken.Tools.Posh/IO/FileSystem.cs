using System.IO;

namespace Hadouken.Tools.Posh.IO {
    public sealed class FileSystem : IFileSystem {
        public Stream OpenRead(string path) {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}