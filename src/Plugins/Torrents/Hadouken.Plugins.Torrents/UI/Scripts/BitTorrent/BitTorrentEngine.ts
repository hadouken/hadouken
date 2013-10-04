///<reference path="../hdkn.d.ts"/>

///<reference path="Torrent.ts"/>

module Hadouken.Plugins.Torrents.BitTorrent {
    export class BitTorrentEngine {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');
        private _torrents: Array<Hadouken.Plugins.Torrents.BitTorrent.Torrent> = new Array<Hadouken.Plugins.Torrents.BitTorrent.Torrent>();

        load(): void {
            this.setupTimer(10);
        }

        setupTimer(interval: number): void {
            setTimeout(() => this.request(), interval);
        }

        request(): void {
            this._rpcClient.call('torrents.list', (torrents: Array<Hadouken.Plugins.Torrents.BitTorrent.Torrent>) => {
                this.setupTimer(1000);
                this.handleRequest(torrents);
            });
        }

        handleRequest(torrents: Array<Hadouken.Plugins.Torrents.BitTorrent.Torrent>): void {
            console.log(torrents);
        }
    }
}