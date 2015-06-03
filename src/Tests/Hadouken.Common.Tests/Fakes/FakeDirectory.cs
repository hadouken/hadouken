using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.IO;

namespace Hadouken.Common.Tests.Fakes {
    public sealed class FakeDirectory : IDirectory {
        private readonly bool _creatable;
        private readonly FakeFileSystem _fileSystem;
        private readonly DirectoryPath _path;

        public FakeDirectory(FakeFileSystem fileSystem, DirectoryPath path, bool creatable) {
            this._fileSystem = fileSystem;
            this._path = path;
            this.Exists = false;
            this._creatable = creatable;
        }

        public DirectoryPath Path {
            get { return this._path; }
        }

        public bool Exists { get; set; }
        public bool Hidden { get; set; }

        public void Create() {
            if (this._creatable) {
                this.Exists = true;
            }
        }

        public void Delete(bool recursive) {
            if (recursive) {
                foreach (var directory in this.GetDirectories("*", SearchScope.Current)) {
                    directory.Delete(true);
                }
                foreach (var file in this.GetFiles("*", SearchScope.Current)) {
                    file.Delete();
                }
            }
            this.Exists = false;
        }

        public IEnumerable<IDirectory> GetDirectories(string filter, SearchScope scope) {
            var children =
                this._fileSystem.Directories.Where(
                    x => x.Key.FullPath.StartsWith(this._path.FullPath + "/", StringComparison.OrdinalIgnoreCase));
            return (from child in children.Where(c => c.Value.Exists) let relative = child.Key.FullPath.Substring(this._path.FullPath.Length + 1) where !relative.Contains("/") select child.Value).Cast<IDirectory>().ToList();
        }

        public IEnumerable<IFile> GetFiles(string filter, SearchScope scope) {
            var children =
                this._fileSystem.Files.Where(
                    x => x.Key.FullPath.StartsWith(this._path.FullPath + "/", StringComparison.OrdinalIgnoreCase));
            return (from child in children.Where(c => c.Value.Exists) let relative = child.Key.FullPath.Substring(this._path.FullPath.Length + 1) where !relative.Contains("/") select child.Value).Cast<IFile>().ToList();
        }

        public IDirectory GetParent() {
            throw new NotImplementedException();
        }
    }
}