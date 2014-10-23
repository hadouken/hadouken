using System;
using Hadouken.Tools.Posh.Models;
using Hadouken.Tools.Posh.Net;

namespace Hadouken.Tools.Posh.Commands
{
    public sealed class RemoveTorrentCommand : IRemoveTorrentCommand
    {
        private readonly IJsonRpcClient _jsonRpcClient;

        public RemoveTorrentCommand(IJsonRpcClient jsonRpcClient)
        {
            if (jsonRpcClient == null) throw new ArgumentNullException("jsonRpcClient");
            _jsonRpcClient = jsonRpcClient;
        }

        public Torrent Torrent { get; set; }

        public bool RemoveData { get; set; }
        
        public void Process(IRuntime runtime)
        {
            _jsonRpcClient.AccessToken = runtime.AccessToken;
            _jsonRpcClient.Url = runtime.Url;

            _jsonRpcClient.Call("torrents.remove", Torrent.InfoHash, RemoveData);
        }
    }
}