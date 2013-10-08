///<reference path="../hdkn.d.ts"/>

///<reference path="../BitTorrent/Torrent.ts"/>
///<reference path="AddTorrentsDialog.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class TorrentsListPage extends Hadouken.UI.Page {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');
        private _eventListener: Hadouken.Events.EventListener;
        private _rows: { [id: string]: any; } = {};
        private _templates: { [id: string]: any; } = {};
        private _timer: any = null;

        constructor() {
            super("/plugins/core.torrents/list.html", [
                '/torrents'
            ]);

            this._eventListener = new Hadouken.Events.EventListener();
        }

        load(): void {
            this._eventListener.addHandler('web.signalR.connected', () => {
                this.loadTemplates();
                this.setupUI();
                this.setupEvents();
                this.setupTorrents();
            });

            this._eventListener.connect();
        }

        unload(): void {
            clearTimeout(this._timer);

            this._eventListener.disconnect();

            var rows = Object.keys(this._rows);

            for (var i = 0; i < rows.length; i++) {
                delete this._rows[rows[i]];
            }
        }

        loadTemplates(): void {
            Handlebars.registerHelper('fileSize', function (size) {
                var i = Math.floor(Math.log(size) / Math.log(1024));
                return (size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
            });

            Handlebars.registerHelper('progress', function (torrent) {
                if (torrent.state === 'Downloading') {
                    var progress = torrent.progress | 0;
                    return ' (' + progress + '%)';
                }

                return '';
            });

            this._templates['tmpl-torrent-list-item'] = Handlebars.compile($('#tmpl-torrent-list-item').html());
        }

        setupUI(): void {
            this.content.find('#btn-show-add-torrents').on('click', (e) => {
                e.preventDefault();
                new Hadouken.Plugins.Torrents.UI.AddTorrentsDialog().show();
            });

            var that = this;

            this.content.find('#tbody-torrents-list').on('click', '.btn-torrent-start', function(e) {
                e.preventDefault();
                var id = $(this).closest('tr').attr('data-torrent-id');
                that.startTorrent(id);
            });
        }

        setupEvents(): void {
            this._eventListener.addHandler('torrent.added', (torrent) => this.torrentAdded(torrent));
            this._eventListener.addHandler('torrent.removed', (id) => this.torrentRemoved(id));
        }

        setupTorrents(): void {
            this._rpcClient.call('torrents.list', (torrents) => {
                for (var i = 0; i < torrents.length; i++) {
                    this.torrentAdded(torrents[i]);
                }

                this.setupIntervalTimer();
            });
        }

        setupIntervalTimer(): void {
            this._timer = setTimeout(() => this.request(), 1000);
        }

        request(): void {
            this._rpcClient.call('torrents.list', (torrents: Array<Hadouken.Plugins.Torrents.BitTorrent.Torrent>) => {
                this.setupIntervalTimer();
                this.update(torrents);
            });
        }

        update(torrents: Array<Hadouken.Plugins.Torrents.BitTorrent.Torrent>): void {
            for (var i = 0; i < torrents.length; i++) {
                var torrent = torrents[i];
                this.torrentUpdated(torrent);
            }
        }

        torrentAdded(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
            // Add new row to table
            console.log('torrentAdded: ' + torrent.id);
            this.addRow(torrent);
        }

        torrentRemoved(id: string): void {
            // Remove row from table
            console.log('torrentRemoved: ' + id);
            this.removeRow(id);
        }

        torrentUpdated(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
            // Update torrent row in table
            console.log('torrentUpdated: ' + torrent.id);
            this.updateRow(torrent);
        }

        private addRow(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
            if (typeof this._rows[torrent.id] !== "undefined")
                return;

            var row = this._templates['tmpl-torrent-list-item']({ torrent: torrent });
            $('#tbody-torrents-list').append($(row));

            this._rows[torrent.id] = row;
        }

        private removeRow(id: string): void {
            //
            if (typeof this._rows[id] === "undefined")
                return;
        }

        private updateRow(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
            if (typeof this._rows[torrent.id] === "undefined")
                return;

            var progress = torrent.progress | 0;

            var row = this.content.find('#tbody-torrents-list > tr[data-torrent-id="' + torrent.id + '"]');
            row.find('.progress-bar').css('width', progress + '%');
            row.find('.state').text(torrent.state);

            if (torrent.state === 'Downloading') {
                row.find('.state-progress').text(progress + '%');
            }
            else {
                row.find('.state-progress').text('');
            }
        }

        private sortTable(column: string): void {
            //
        }

        startTorrent(id: string): void {
            this._rpcClient.callParams('torrents.start', id, (result) => {
                if (result) {
                    console.log('started');
                }
            });
        }
    }
}