using System;
using Hadouken.Tools.Posh.Models;
using Hadouken.Tools.Posh.Net;

namespace Hadouken.Tools.Posh.Commands
{
    public sealed class GetTorrentCommand : IGetTorrentCommand
    {
        private readonly IJsonRpcClient _jsonRpcClient;

        public GetTorrentCommand(IJsonRpcClient jsonRpcClient)
        {
            if (jsonRpcClient == null) throw new ArgumentNullException("jsonRpcClient");
            _jsonRpcClient = jsonRpcClient;
        }

        public void Process(IRuntime runtime)
        {
            _jsonRpcClient.AccessToken = runtime.AccessToken;
            _jsonRpcClient.Url = runtime.Url;

            var torrents = _jsonRpcClient.Call<Torrent[]>("torrents.getAll");

            runtime.WriteObject(torrents, true);
        }
    }
}