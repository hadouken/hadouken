///<reference path="../hdkn.d.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class ConfigureDialog extends Hadouken.UI.Dialog {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');

        constructor() {
            super("/plugins/core.torrents/dialogs/configure.html");
        }

        onShow(): void {
            this.setupEvents();
            this.loadData();
        }

        setupEvents(): void {
            this.getContent().find('#tab-menu a').on('click', function (e) {
                e.preventDefault();
                $(this).tab('show');
            });

            this.getContent().find('#btn-save-torrent-config').on('click', (e) => {
                e.preventDefault();
                this.saveData();
            });
        }

        loadData(): void {
            var d = [['bt.downloads.savePath', 'bt.connection.listenPort']];
            this._rpcClient.callParams('config.getMany', d, (result) => {
                this.getContent().find('#savePath').val(result['bt.downloads.savePath']);
                this.getContent().find('#port').val(result['bt.connection.listenPort']);
            });
        }

        saveData(): void {
            var data = {
                'bt.downloads.savePath': this.getContent().find('#savePath').val(),
                'bt.connection.listenPort': this.getContent().find('#port').val()
            };

            this._rpcClient.callParams('config.setMany', data, (result) => {
                console.log('saved');
            });
        }
    }
}