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
            this._eventListener.addHandler("torrent.added", (t: Hadouken.Plugins.Torrents.BitTorrent.Torrent) => this.added(t));
            this._eventListener.addHandler("torrent.removed", (id: string) => this.removed(id));

            // Get a list of all existing torrents
            this._rpcClient.call('torrents.list', (torrents: Array<Hadouken.Plugins.Torrents.BitTorrent.Torrent>) => {
                for (var i = 0; i < torrents.length; i++) {
                    var torrent = torrents[i];
                    this._torrents[torrent.id] = torrent;
                }

                this.setupTimer(1000);
            });
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

                // Only update torrents which we have locally
                if (typeof this._torrents[torrent.id] === "undefined")
                    continue;

                this._torrents[torrent.id] = torrent;
                this._eventListener.sendEvent('torrent.updated', torrent);
            }
        }

        added(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
            this._eventListener.sendEvent('torrent.added', torrent);
            this._torrents[torrent.id] = torrent;
        }

        removed(id: string): void {
            delete this._torrents[id];
            this._eventListener.sendEvent('torrent.removed', id);
        }
    }
}