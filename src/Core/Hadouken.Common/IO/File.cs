using System.IO;

namespace Hadouken.Common.IO {
    internal sealed class File : IFile {
        private readonly FileInfo _file;
        private readonly FilePath _path;

        public File(FilePath path) {
            this._path = path;
            this._file = new FileInfo(path.FullPath);
        }

        public FilePath Path {
            get { return this._path; }
        }

        public bool Exists {
            get { return this._file.Exists; }
        }

        public long Length {
            get { return this._file.Length; }
        }

        public void Copy(FilePath destination, bool overwrite) {
            this._file.CopyTo(destination.FullPath, overwrite);
        }

        public void Move(FilePath destination) {
            this._file.MoveTo(destination.FullPath);
        }

        public void Delete() {
            this._file.Delete();
        }

        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare) {
            return this._file.Open(fileMode, fileAccess, fileShare);
        }
    }
}