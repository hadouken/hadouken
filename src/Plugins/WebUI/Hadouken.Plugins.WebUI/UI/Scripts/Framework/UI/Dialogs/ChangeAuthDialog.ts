///<reference path="../Dialog.ts"/>
///<reference path="../../Http/JsonRpcClient.ts"/>

module Hadouken.UI.Dialogs {
    export class ChangeAuthDialog extends Hadouken.UI.Dialog {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');

        constructor() {
            super('/dialogs/change-auth.html');
        }

        onShow(): void {
            var content = this.getContent();

            this._rpcClient.call('core.getAuthInfo', (d) => {
                content.find('#auth-username').val(d.username);
            });

            content.find('#btn-save-auth').on('click', (e) => {
                e.preventDefault();
                this.save();
            });
        }

        save(): void {
            var content = this.getContent();
            var username = content.find('#auth-username').val();
            var newPassword = content.find('#auth-newPassword').val();

            var d = [username, newPassword, ''];

            this._rpcClient.callParams('core.setAuth', d, (r) => {
                if (!r) {
                    alert('Failed to set auth.');
                }
                else {
                    this.close();
                }
            });
        }
    }
}