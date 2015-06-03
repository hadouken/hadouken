using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hadouken.Common.IO {
    internal sealed class Directory : IDirectory {
        private readonly DirectoryInfo _directory;
        private readonly DirectoryPath _path;

        public Directory(DirectoryPath path) {
            this._path = path;
            this._directory = new DirectoryInfo(this._path.FullPath);
        }

        public DirectoryPath Path {
            get { return this._path; }
        }

        public bool Exists {
            get { return this._directory.Exists; }
        }

        public bool Hidden {
            get { return this._directory.Attributes.HasFlag(FileAttributes.Hidden); }
        }

        public void Create() {
            this._directory.Create();
        }

        public void Delete(bool recursive) {
            this._directory.Delete(recursive);
        }

        public IEnumerable<IDirectory> GetDirectories(string filter, SearchScope scope) {
            var option = scope == SearchScope.Current ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
            return this._directory.GetDirectories(filter, option).Select(directory => new Directory(directory.FullName));
        }

        public IEnumerable<IFile> GetFiles(string filter, SearchScope scope) {
            var option = scope == SearchScope.Current ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
            return this._directory.GetFiles(filter, option).Select(file => new File(file.FullName));
        }

        public IDirectory GetParent() {
            if (this._directory.Parent == null) {
                return null;
            }
            return new Directory(this._directory.Parent.FullName);
        }
    }
}