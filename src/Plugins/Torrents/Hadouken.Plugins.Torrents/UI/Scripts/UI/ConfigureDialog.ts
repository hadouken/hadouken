///<reference path="../hdkn.d.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class ConfigureDialog extends Hadouken.UI.Dialog {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');

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

            this.getContent().find('#btn-save-torrent-config').on('click', (e) => {
                e.preventDefault();

                var data = {
                    'bt.downloads.savePath': this.getContent().find('#savePath').val(),
                    'bt.connection.listenPort': this.getContent().find('#port').val()
                };

                this._rpcClient.callParams('config.setMany', data, (result) => {
                    console.log('saved');
                });
            });
        }
    }
}