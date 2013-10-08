///<reference path="../hdkn.d.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class ConfigureDialog extends Hadouken.UI.Dialog {
        constructor() {
            super("/plugins/core.torrents/dialogs/configure.html");
        }

        onShow(): void {
            this.setupEvents();
        }

        setupEvents(): void {
            this.getContent().find('#tab-menu a').on('click', function (e) {
                e.preventDefault();
                $(this).tab('show');
            });
        }
    }
}