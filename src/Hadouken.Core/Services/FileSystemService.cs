using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.IO;
using Hadouken.Common.JsonRpc;

namespace Hadouken.Core.Services
{
    public sealed class FileSystemService : IJsonRpcService
    {
        private readonly IFileSystem _fileSystem;

        public FileSystemService(IFileSystem fileSystem)
        {
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");
            _fileSystem = fileSystem;
        }

        [JsonRpcMethod("fileSystem.getDirectories")]
        public IEnumerable<string> GetDirectories(string path)
        {
            var currentDirectory = _fileSystem.GetDirectory(path);
            return currentDirectory.GetDirectories("*", SearchScope.Current)
                .Where(d => !d.Hidden)
                .Select(d => d.Path.FullPath);
        }

        [JsonRpcMethod("fileSystem.getParent")]
        public string GetParent(string path)
        {
            var currentDirectory = _fileSystem.GetDirectory(path);
            return currentDirectory.GetParent().Path.FullPath;
        }

        [JsonRpcMethod("fileSystem.getDrives")]
        public IEnumerable<IDriveInfo> GetDrives()
        {
            return _fileSystem.GetDrives().Where(drive => drive.IsReady);
        } 
    }
}
