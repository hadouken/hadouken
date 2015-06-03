using System;
using Hadouken.Tools.Posh.Models;
using Hadouken.Tools.Posh.Net;

namespace Hadouken.Tools.Posh.Commands {
    public sealed class RemoveTorrentCommand : IRemoveTorrentCommand {
        private readonly IJsonRpcClient _jsonRpcClient;

        public RemoveTorrentCommand(IJsonRpcClient jsonRpcClient) {
            if (jsonRpcClient == null) {
                throw new ArgumentNullException("jsonRpcClient");
            }
            this._jsonRpcClient = jsonRpcClient;
        }

        public Torrent Torrent { get; set; }
        public bool RemoveData { get; set; }

        public void Process(IRuntime runtime) {
            this._jsonRpcClient.AccessToken = runtime.AccessToken;
            this._jsonRpcClient.Url = runtime.Url;

            this._jsonRpcClient.Call("torrents.remove", this.Torrent.InfoHash, this.RemoveData);
        }
    }
}