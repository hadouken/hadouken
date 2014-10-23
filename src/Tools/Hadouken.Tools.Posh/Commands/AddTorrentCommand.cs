using System;
using Hadouken.Tools.Posh.IO;
using Hadouken.Tools.Posh.Net;

namespace Hadouken.Tools.Posh.Commands
{
    public class AddTorrentCommand : IAddTorrentCommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonRpcClient _jsonRpcClient;

        public AddTorrentCommand(IFileSystem fileSystem, IJsonRpcClient jsonRpcClient)
        {
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");
            if (jsonRpcClient == null) throw new ArgumentNullException("jsonRpcClient");
            _fileSystem = fileSystem;
            _jsonRpcClient = jsonRpcClient;
        }

        public string Path { get; set; }

        public string SavePath { get; set; }
        
        public void Process(IRuntime runtime)
        {
            _jsonRpcClient.AccessToken = runtime.AccessToken;
            _jsonRpcClient.Url = runtime.Url;

            var filePaths = runtime.GetResolvedPaths(Path);

            foreach (var filePath in filePaths)
            {
                ProcessFile(filePath);
            }
        }

        private void ProcessFile(string path)
        {
            using (var file = _fileSystem.OpenRead(path))
            {
                var data = new byte[file.Length];
                file.Read(data, 0, (int)file.Length);

                AddFile(data, SavePath, string.Empty);
            }
        }

        private void AddFile(byte[] data, string savePath, string label)
        {
            _jsonRpcClient.Call("torrents.addFile",
                    Convert.ToBase64String(data),
                    savePath,
                    string.Empty);
        }
    }
}