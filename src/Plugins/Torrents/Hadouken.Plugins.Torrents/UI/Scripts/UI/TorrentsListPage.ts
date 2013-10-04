///<reference path="../hdkn.d.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class TorrentsListPage extends Hadouken.UI.Page {
        constructor() {
            super("/plugins/core.torrents/list.html");
        }

        setup(): void {
            this.setupEvents();
        }

        setupEvents(): void {

        }
    }
}