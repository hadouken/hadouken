using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.IO.Local
{
    public class LocalDirectory : IDirectory
    {
        private readonly DirectoryInfo _directoryInfo;

        public LocalDirectory(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public string FullPath
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public IDirectory[] Directories
        {
            get { throw new NotImplementedException(); }
        }

        public IFile[] Files
        {
            get { return _directoryInfo.GetFiles().Select(f => new LocalFile(f)).ToArray(); }
        }

        public bool Exists
        {
            get { return _directoryInfo.Exists; }
        }

        public void Create()
        {
            _directoryInfo.Create();
        }

        public void CreateIfNotExists()
        {
            if (!Exists)
                Create();
        }

        public void Delete()
        {
            _directoryInfo.Delete();
        }
    }
}
