///<reference path="../hdkn.d.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class ConfigureStep extends Hadouken.UI.WizardStep {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');

        constructor() {
            super('/plugins/core.torrents/wizards/first-time-setup.html', 'Torrents');
        }

        public loadData(callback: any): void {
            console.log('loading data for torrents');
            setTimeout(() => callback(), 2000);
        }

        public saveData(callback: any): void {
            console.log('saving data for torrents');
            setTimeout(() => callback(), 1000);
        }
    }
}