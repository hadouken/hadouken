///<reference path="../hdkn.d.ts"/>

///<reference path="../BitTorrent/Torrent.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class TorrentDetailsPage extends Hadouken.UI.Page {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');

        constructor() {
            super('/plugins/core.torrents/details.html', [
                '/torrents/{id}'
            ]);
        }

        load(...args: any[]): void {
            if (typeof args[0] === "undefined") {
                this.showNotFound();
                return;
            }

            this.loadTorrentDetails(args[0]);
        }

        loadTorrentDetails(id: string): void {
            this._rpcClient.callParams('torrents.details', id, (torrent) => {
                this.loadUI(torrent);
            });
        }

        loadUI(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
            $('#torrent-details-name').text(torrent.name);
        }

        friendlyFileSize(size: number): string {
            var i = Math.floor(Math.log(size) / Math.log(1024));
            return (size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
        }

        showNotFound(): void {
        }
    }
}