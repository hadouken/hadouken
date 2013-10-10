///<reference path="../hdkn.d.ts"/>

///<reference path="../BitTorrent/Torrent.ts"/>
///<reference path="AddTorrentsDialog.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class TorrentsListPage extends Hadouken.UI.Page {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');
        private _eventListener: Hadouken.Events.EventListener;
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
        }

        loadTemplates(): void {
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

            this.content.find('#tbody-torrents-list').on('click', '.btn-torrent-start', function (e) {
                e.preventDefault();
                var id = $(this).closest('tr').attr('data-torrent-id');
                that.startTorrent(id);
            });

            this.content.find('#tbody-torrents-list').on('click', '.btn-torrent-pause', function (e) {
                e.preventDefault();
                var id = $(this).closest('tr').attr('data-torrent-id');
                that.pauseTorrent(id);
            });

            this.content.find('#tbody-torrents-list').on('click', '.btn-torrent-stop', function (e) {
                e.preventDefault();
                var id = $(this).closest('tr').attr('data-torrent-id');
                that.stopTorrent(id);
            });
        }

        setupEvents(): void {
            this._eventListener.addHandler('torrent.added', (torrent) => this.torrentAdded(torrent));
            this._eventListener.addHandler('torrent.paused', (torrent) => this.updateRow(torrent));
            this._eventListener.addHandler('torrent.started', (torrent) => this.updateRow(torrent));
            this._eventListener.addHandler('torrent.stopped', (torrent) => this.updateRow(torrent));
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
            this.addRow(torrent);
            this.updateRow(torrent);
        }

        torrentRemoved(id: string): void {
            this.removeRow(id);
        }

        torrentUpdated(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
            this.updateRow(torrent);
        }

        private addRow(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
            var row = this._templates['tmpl-torrent-list-item']({ torrent: torrent });
            $('#tbody-torrents-list').append($(row));
        }

        private removeRow(id: string): void {
            var row = this.content.find('#tbody-torrents-list > tr[data-torrent-id="' + id + '"]');
            row.remove();
        }

        private updateRow(torrent: Hadouken.Plugins.Torrents.BitTorrent.Torrent): void {
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


            row.find('.btn-torrent-start').attr('disabled', !this.canStart(torrent.state));
            row.find('.btn-torrent-pause').attr('disabled', !this.canPause(torrent.state));
            row.find('.btn-torrent-stop').attr('disabled', !this.canStop(torrent.state));
        }

        private canStart(state: string): boolean {
            if (state === 'Stopped' || state === 'Paused')
                return true;

            return false;
        }

        private canPause(state: string): boolean {
            if (state === 'Downloading' || state === 'Seeding') {
                return true;
            }

            return false;
        }

        private canStop(state: string): boolean {
            if (state === 'Downloading' || state === 'Paused' || state === 'Seeding' || state === 'Error' || state === 'Hashing' || state === 'Metadata')
                return true;

            return false;
        }

        private sortTable(column: string): void {
            //
        }

        startTorrent(id: string): void {
            this._rpcClient.callParams('torrents.start', id, (result) => {
            });
        }

        pauseTorrent(id: string): void {
            this._rpcClient.callParams('torrents.pause', id, (result) => {
            });
        }

        stopTorrent(id: string): void {
            this._rpcClient.callParams('torrents.stop', id, (result) => {
            });
        }
    }
}