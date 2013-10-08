///<reference path="../hdkn.d.ts"/>

///<reference path="../BitTorrent/Torrent.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class TorrentDetailsPage extends Hadouken.UI.Page {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');
        private _timer: any;

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

        unload(): void {
            clearTimeout(this._timer);
        }

        loadTorrentDetails(id: string): void {
            this._rpcClient.callParams('torrents.details', id, (torrent) => {
                this.refreshUI(torrent);
                this._timer = setTimeout(() => this.loadTorrentDetails(id), 1000);
            });
        }

        refreshUI(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
            var progress = torrent.progress | 0;

            $('#torrent-details-name').text(torrent.name + ' ');
            $('#torrent-details-name').append('<small>' + torrent.state + '</small>');
            $('#torrent-details-progress').css('width', progress + '%');
        }

        friendlyFileSize(size: number): string {
            var i = Math.floor(Math.log(size) / Math.log(1024));
            return (size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
        }

        showNotFound(): void {
        }
    }
}