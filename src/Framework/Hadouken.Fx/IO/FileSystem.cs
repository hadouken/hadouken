using System.IO;

namespace Hadouken.Fx.IO
{
    public class FileSystem : IFileSystem
    {
        public IDirectory GetDirectory(string path)
        {
            return new Directory(new DirectoryInfo(path));
        }

        public IFile GetFile(string path)
        {
            return new File(new FileInfo(path));
        }
    }
}
