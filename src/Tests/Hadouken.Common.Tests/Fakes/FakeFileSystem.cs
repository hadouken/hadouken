using System;
using System.Collections.Generic;
using System.IO;
using Hadouken.Common.IO;

namespace Hadouken.Common.Tests.Fakes {
    public sealed class FakeFileSystem : IFileSystem {
        private readonly Dictionary<DirectoryPath, FakeDirectory> _directories;
        private readonly Dictionary<FilePath, FakeFile> _files;

        public FakeFileSystem(bool isUnix) {
            this._directories = new Dictionary<DirectoryPath, FakeDirectory>(new PathComparer(isUnix));
            this._files = new Dictionary<FilePath, FakeFile>(new PathComparer(isUnix));
        }

        public Dictionary<DirectoryPath, FakeDirectory> Directories {
            get { return this._directories; }
        }

        public Dictionary<FilePath, FakeFile> Files {
            get { return this._files; }
        }

        public IFile GetFile(FilePath path) {
            if (!this.Files.ContainsKey(path)) {
                this.Files.Add(path, new FakeFile(this, path));
            }
            return this.Files[path];
        }

        public IDirectory GetDirectory(DirectoryPath path) {
            return this.GetDirectory(path, true);
        }

        public IEnumerable<IDriveInfo> GetDrives() {
            throw new NotImplementedException();
        }

        public IFile GetCreatedFile(FilePath path) {
            var file = this.GetFile(path);
            file.Open(FileMode.Create, FileAccess.Write, FileShare.None).Close();
            return file;
        }

        public IFile GetCreatedFile(FilePath path, string content) {
            var file = this.GetFile(path);
            var stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None);
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Close();
            stream.Close();
            return file;
        }

        public void DeleteDirectory(DirectoryPath path) {
            if (this.Directories.ContainsKey(path)) {
                this.Directories[path].Exists = false;
            }
        }

        public IDirectory GetCreatedDirectory(DirectoryPath path) {
            var directory = this.GetDirectory(path, true);
            directory.Create();
            return directory;
        }

        public IDirectory GetNonCreatableDirectory(DirectoryPath path) {
            return this.GetDirectory(path, false);
        }

        private IDirectory GetDirectory(DirectoryPath path, bool creatable) {
            if (!this.Directories.ContainsKey(path)) {
                this.Directories.Add(path, new FakeDirectory(this, path, creatable));
            }
            return this.Directories[path];
        }

        public string GetTextContent(FilePath path) {
            var file = this.GetFile(path) as FakeFile;
            if (file == null) {
                throw new InvalidOperationException();
            }

            try {
                if (file.Deleted) {
                    this.Files[path].Exists = true;
                }

                using (var stream = file.OpenRead()) {
                    using (var reader = new StreamReader(stream)) {
                        return reader.ReadToEnd();
                    }
                }
            }
            finally {
                if (file.Deleted) {
                    this.Files[path].Exists = false;
                }
            }
        }
    }
}