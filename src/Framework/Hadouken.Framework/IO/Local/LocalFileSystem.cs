using System.IO;

namespace Hadouken.Framework.IO.Local
{
    public class LocalFileSystem : IFileSystem
    {
        private readonly IRootPathProvider _rootPathProvider;

        public LocalFileSystem(IRootPathProvider rootPathProvider)
        {
            _rootPathProvider = rootPathProvider;
        }

        public string RootPath
        {
            get { return _rootPathProvider.GetRootPath(); }
        }

        public IDirectory GetDirectory(string path)
        {
            return new LocalDirectory(new DirectoryInfo(path));
        }

        public IFile GetFile(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}
