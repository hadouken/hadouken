///<reference path="../hdkn.d.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class ConfigureStep extends Hadouken.UI.WizardStep {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');

        constructor() {
            super('/plugins/core.torrents/wizards/first-time-setup.html', 'Torrents');
        }

        public loadData(callback: any): void {
            this._rpcClient.callParams('config.get', 'bt.downloads.savePath', (r) => {
                this.content.find('#torrents-savePath').val(r);
                callback();
            });
        }

        public saveData(callback: any): void {
            var data = [
                'bt.downloads.savePath', this.content.find('#torrents-savePath').val()
            ];

            this._rpcClient.callParams('config.set', data, () => {
                callback();
            });
        }
    }
}