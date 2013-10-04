///<reference path="../hdkn.d.ts"/>

///<reference path="AddTorrentsDialog.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class TorrentsListPage extends Hadouken.UI.Page {
        private _eventListener: Hadouken.Events.EventListener;

        constructor(eventListener: Hadouken.Events.EventListener) {
            super("/plugins/core.torrents/list.html");
            this._eventListener = eventListener;
        }

        setup(): void {
            this.setupUI();
            this.setupEvents();
        }

        setupUI(): void {
            this.getContent().find('#btn-show-add-torrents').on('click', (e) => {
                e.preventDefault();
                new Hadouken.Plugins.Torrents.UI.AddTorrentsDialog().show();
            });
        }

        setupEvents(): void {
            this._eventListener.addHandler('web.torrent.added', this.torrentAdded);
        }

        torrentAdded(d): void {
            console.log('kekekekekek');
        }
    }
}