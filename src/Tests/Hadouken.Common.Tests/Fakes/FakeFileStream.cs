using System;
using System.IO;

namespace Hadouken.Common.Tests.Fakes {
    public sealed class FakeFileStream : Stream {
        private readonly FakeFile _file;
        private long _position;

        public FakeFileStream(FakeFile file) {
            this._file = file;
            this._position = 0;
        }

        public override bool CanRead {
            get { return true; }
        }

        public override bool CanSeek {
            get { return true; }
        }

        public override bool CanWrite {
            get { return true; }
        }

        public override long Length {
            get {
                lock (this._file.ContentLock) {
                    return this._file.ContentLength;
                }
            }
        }

        public override long Position {
            get { return this._position; }
            set { this.Seek(value, SeekOrigin.Begin); }
        }

        public override void Flush() {}

        public override int Read(byte[] buffer, int offset, int count) {
            lock (this._file.ContentLock) {
                var end = this._position + count;
                var fileSize = this._file.ContentLength;
                var maxLengthToRead = end > fileSize ? fileSize - this._position : count;
                Buffer.BlockCopy(this._file.Content, (int) this._position, buffer, offset, (int) maxLengthToRead);
                this._position += maxLengthToRead;
                return (int) maxLengthToRead;
            }
        }

        public override long Seek(long offset, SeekOrigin origin) {
            switch (origin) {
                case SeekOrigin.Begin:
                    return this.MoveTo(offset);
                case SeekOrigin.Current:
                    return this.MoveTo(this._position + offset);
                case SeekOrigin.End:
                    return this.MoveTo(this._file.ContentLength - offset);
            }
            throw new NotSupportedException();
        }

        public override void SetLength(long value) {
            lock (this._file.ContentLock) {
                this._file.Resize(value);
            }
        }

        public override void Write(byte[] buffer, int offset, int count) {
            lock (this._file.ContentLock) {
                var fileSize = this._file.ContentLength;
                var endOfWrite = (this._position + count);
                if (endOfWrite > fileSize) {
                    this._file.Resize(endOfWrite);
                }
                Buffer.BlockCopy(buffer, offset, this._file.Content, (int) this._position, count);
                this._position = this._position + count;
            }
        }

        private long MoveTo(long offset) {
            lock (this._file.ContentLock) {
                if (offset < 0) {
                    throw new InvalidOperationException();
                }
                if (offset > this._file.ContentLength) {
                    this._file.Resize(offset);
                }
                this._position = offset;
                return offset;
            }
        }
    }
}