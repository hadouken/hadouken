using System;
using System.IO;
using Hadouken.Common.IO;

namespace Hadouken.Common.Tests.Fakes {
    public sealed class FakeFile : IFile {
        private readonly object _contentLock = new object();
        private readonly FakeFileSystem _fileSystem;
        private readonly FilePath _path;
        private byte[] _content = new byte[4096];

        public FakeFile(FakeFileSystem fileSystem, FilePath path) {
            this._fileSystem = fileSystem;
            this._path = path;
            this.Exists = false;
        }

        public bool Deleted { get; private set; }

        public object ContentLock {
            get { return this._contentLock; }
        }

        public long ContentLength {
            get { return this.Length; }
            set { this.Length = value; }
        }

        public byte[] Content {
            get { return this._content; }
            set { this._content = value; }
        }

        public FilePath Path {
            get { return this._path; }
        }

        public bool Exists { get; set; }
        public long Length { get; private set; }

        public void Copy(FilePath destination, bool overwrite) {
            var file = this._fileSystem.GetCreatedFile(destination) as FakeFile;
            if (file == null) {
                return;
            }
            file.Content = this.Content;
            file.ContentLength = this.ContentLength;
        }

        public void Move(FilePath destination) {
            throw new NotImplementedException();
        }

        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare) {
            var position = this.GetPosition(fileMode);
            var stream = new FakeFileStream(this) {Position = position};
            return stream;
        }

        public void Delete() {
            this.Exists = false;
            this.Deleted = true;
        }

        public void Resize(long offset) {
            if (this.Length < offset) {
                this.Length = offset;
            }
            if (this._content.Length >= this.Length) {
                return;
            }

            var buffer = new byte[this.Length*2];
            Buffer.BlockCopy(this._content, 0, buffer, 0, this._content.Length);
            this._content = buffer;
        }

        private long GetPosition(FileMode fileMode) {
            if (this.Exists) {
                switch (fileMode) {
                    case FileMode.CreateNew:
                        throw new IOException();
                    case FileMode.Create:
                    case FileMode.Truncate:
                        this.Length = 0;
                        return 0;
                    case FileMode.Open:
                    case FileMode.OpenOrCreate:
                        return 0;
                    case FileMode.Append:
                        return this.Length;
                }
            }
            else {
                switch (fileMode) {
                    case FileMode.Create:
                    case FileMode.Append:
                    case FileMode.CreateNew:
                    case FileMode.OpenOrCreate:
                        this.Exists = true;
                        return this.Length;
                    case FileMode.Open:
                    case FileMode.Truncate:
                        throw new FileNotFoundException();
                }
            }
            throw new NotSupportedException();
        }
    }
}