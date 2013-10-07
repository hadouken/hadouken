///<reference path="../hdkn.d.ts"/>

///<reference path="../BitTorrent/Torrent.ts"/>
///<reference path="AddTorrentsDialog.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class TorrentsListPage extends Hadouken.UI.Page {
        private _eventListener: Hadouken.Events.EventListener;
        private _rows: { [id: string]: any; } = {};
        private _templates: { [id: string]: any; } = {};

        constructor(eventListener: Hadouken.Events.EventListener) {
            super("/plugins/core.torrents/list.html", [ '/torrents' ]);
            this._eventListener = eventListener;
        }

        load(): void {
            this.loadTemplates();
            this.setupUI();
            this.setupEvents();
        }

        loadTemplates(): void {
            this._templates['tmpl-torrent-list-item'] = Handlebars.compile($('#tmpl-torrent-list-item').html());
        }

        setupUI(): void {
            this.getContent().find('#btn-show-add-torrents').on('click', (e) => {
                e.preventDefault();
                new Hadouken.Plugins.Torrents.UI.AddTorrentsDialog().show();
            });
        }

        setupEvents(): void {
            this._eventListener.addHandler('torrent.added', (torrent) => this.torrentAdded(torrent));
            this._eventListener.addHandler('torrent.removed', (id) => this.torrentRemoved(id));
            this._eventListener.addHandler('web.torrent.updated', (torrent) => this.torrentUpdated(torrent));
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
            //
            if (typeof this._rows[torrent.id] === "undefined")
                return;
        }

        private sortTable(column: string): void {
            //
        }
    }
}