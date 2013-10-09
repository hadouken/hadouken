///<reference path="../hdkn.d.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class ConfigureDialog extends Hadouken.UI.Dialog {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');

        _keys: Array<string> = [];

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
            this._rpcClient.callParams('config.getSection', 'bt.', (result) => {
                this._keys = Object.keys(result);
                var content = this.getContent();

                for (var i = 0; i < this._keys.length; i++) {
                    var key = this._keys[i];
                    var htmlKey = key.replace(/\./g, '-');
                    
                    content.find('#' + htmlKey).val(result[key]);
                }
            });
        }

        saveData(): void {
            var data = {};
            var content = this.getContent();

            for (var i = 0; i < this._keys.length; i++) {
                var key = this._keys[i];
                var htmlKey = key.replace(/\./g, '-');
                var val = content.find('#' + htmlKey).val();

                data[key] = val;
            }

            this._rpcClient.callParams('config.setMany', data, (result) => {
                if (result) {
                    this.close();
                }
            });
        }
    }
}