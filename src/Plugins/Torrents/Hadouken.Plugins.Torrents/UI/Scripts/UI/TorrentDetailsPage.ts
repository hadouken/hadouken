///<reference path="../hdkn.d.ts"/>

///<reference path="../BitTorrent/Torrent.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class TorrentDetailsPage extends Hadouken.UI.Page {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');
        private _timer: any;
        private _hasRenderedTemplate: boolean = false;

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
            this._hasRenderedTemplate = false;
            clearTimeout(this._timer);
        }

        loadTorrentDetails(id: string): void {
            this._rpcClient.callParams('torrents.details', id, (torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent) => {
                if (!this._hasRenderedTemplate) {
                    this.renderFileTable(torrent.files);
                    this._hasRenderedTemplate = true;
                }

                this.refreshUI(torrent);
                this._timer = setTimeout(() => this.loadTorrentDetails(id), 1000);
            });
        }

        renderFileTable(files: Array<Hadouken.Plugins.Torrents.BitTorrent.TorrentFile>): void {
            Handlebars.registerHelper('fileSize', (size) => {
                return this.friendlyFileSize(size);
            });

            var tmpl = Handlebars.compile($('#tmpl-torrentdetails-file-item').html());

            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                var row = tmpl({ file: file });

                $('#tbody-torrentfiles-list').append($(row));
            }
        }

        refreshUI(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
            var progress = torrent.progress | 0;

            $('#torrent-details-name').text(torrent.name + ' ');
            $('#torrent-details-name').append('<small>' + torrent.state + '</small>');
            $('#torrent-details-progress').css('width', progress + '%');

            for (var i = 0; i < torrent.files.length; i++) {
                var file = torrent.files[i];
                var row = $('#tbody-torrentfiles-list > tr[data-file-index="' + file.index + '"]');
            }
        }

        friendlyFileSize(size: number): string {
            var i = Math.floor(Math.log(size) / Math.log(1024));
            return (size / Math.pow(1024, i)).toFixed(2) + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
        }

        showNotFound(): void {
        }
    }
}