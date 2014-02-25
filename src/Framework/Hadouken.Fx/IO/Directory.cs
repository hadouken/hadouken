using System.IO;
using System.Linq;

namespace Hadouken.Fx.IO
{
    public class Directory : IDirectory
    {
        private readonly DirectoryInfo _directoryInfo;

        public Directory(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public IDirectory[] Directories
        {
            get { return GetDirectories(); }
        }

        public IFile[] Files
        {
            get { return GetFiles(); }
        }

        public string FullPath
        {
            get { return _directoryInfo.FullName; }
        }

        private IDirectory[] GetDirectories()
        {
            var directories = _directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
            return (from directory in directories select new Directory(directory) as IDirectory).ToArray();
        }

        private IFile[] GetFiles()
        {
            var files = _directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
            return (from file in files select new File(file) as IFile).ToArray();
        }
    }
}
