///<reference path="../hdkn.d.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class AddTorrentsDialog extends Hadouken.UI.Dialog {
        constructor() {
            super("/plugins/core.torrents/dialogs/add.html");
        }

        onShow(): void {
            this.setupEvents();
        }

        setupEvents(): void {
            var c = this.getContent();

            c.find('#btn-add-torrents').on('click', (e) => {
                e.preventDefault();
                this.addTorrents();
            });
        }

        addTorrents(): void {
            console.log('adding torrents');
        }
    }
}