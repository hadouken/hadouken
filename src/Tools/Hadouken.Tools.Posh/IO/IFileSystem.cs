using System.IO;

namespace Hadouken.Tools.Posh.IO {
    public interface IFileSystem {
        Stream OpenRead(string path);
    }
}