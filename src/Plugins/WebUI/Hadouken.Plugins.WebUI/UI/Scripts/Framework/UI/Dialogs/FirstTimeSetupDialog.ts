///<reference path="../Dialog.ts"/>
///<reference path="../../Http/JsonRpcClient.ts"/>

module Hadouken.UI.Dialogs {
    export class FirstTimeSetupDialog extends Hadouken.UI.Dialog {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');

        constructor() {
            super('/dialogs/first-time-setup.html');
        }

        onShow(): void {
        }

        onClosed() {
            this._rpcClient.callParams('config.set', [ 'web.firstTimeSetupShown', true ], (c) => { });
        }
    }
}