///<reference path="../hdkn.d.ts"/>

///<reference path="AddTorrentsDialog.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class TorrentsListPage extends Hadouken.UI.Page {
        constructor() {
            super("/plugins/core.torrents/list.html");
        }

        setup(): void {
            this.setupEvents();
        }

        setupEvents(): void {
            this.getContent().find('#btn-show-add-torrents').on('click', (e) => {
                e.preventDefault();
                new Hadouken.Plugins.Torrents.UI.AddTorrentsDialog().show();
            });
        }
    }
}