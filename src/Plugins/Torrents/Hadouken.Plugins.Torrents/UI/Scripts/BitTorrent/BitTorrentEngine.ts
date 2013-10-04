///<reference path="../hdkn.d.ts"/>

///<reference path="Torrent.ts"/>

module Hadouken.Plugins.Torrents.BitTorrent {
    export class BitTorrentEngine {
        private _eventListener: Hadouken.Events.EventListener;
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');
        private _torrents: { [id: string]: Hadouken.Plugins.Torrents.BitTorrent.Torrent; } = {};

        constructor(eventListener: Hadouken.Events.EventListener) {
            this._eventListener = eventListener;
        }

        load(): void {
            this._eventListener.addHandler("torrent.added", this.added);
            this._eventListener.addHandler("torrent.removed", this.removed);

            this.setupTimer(10);
        }

        setupTimer(interval: number): void {
            setTimeout(() => this.request(), interval);
        }

        request(): void {
            this._rpcClient.call('torrents.list', (torrents: Array<Hadouken.Plugins.Torrents.BitTorrent.Torrent>) => {
                this.setupTimer(1000);
                this.update(torrents);
            });
        }

        update(torrents: Array<Hadouken.Plugins.Torrents.BitTorrent.Torrent>): void {
            for (var i = 0; i < torrents.length; i++) {
                var torrent = torrents[i];
                this._torrents[torrent.id] = torrent;
            }
        }

        added(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
            this._torrents[torrent.id] = torrent;
            this._eventListener.sendEvent('torrent.added', torrent);
        }

        removed(id: string): void {
            delete this._torrents[id];
            this._eventListener.sendEvent('torrent.removed', id);
        }
    }
}