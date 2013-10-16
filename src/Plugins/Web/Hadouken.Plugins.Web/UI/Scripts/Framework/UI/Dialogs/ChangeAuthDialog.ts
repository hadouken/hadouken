///<reference path="../Dialog.ts"/>
///<reference path="../Overlay.ts"/>
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
                content.find('#auth-username').val(d.userName);
            });

            content.find('#auth-username').focus();

            content.find('#btn-save-auth').on('click', (e) => {
                e.preventDefault();
                this.save();
            });
        }

        save(): void {
            var content = this.getContent();

            // Show overlay
            var overlay = new Hadouken.UI.Overlay('icon-refresh loading');
            overlay.show(content.find('.modal-body'));

            var username = content.find('#auth-username').val();
            var newPassword = content.find('#auth-newPassword').val();
            var currentPassword = content.find('#auth-currentPassword').val();

            var d = [username, newPassword, currentPassword];

            this._rpcClient.callParams('core.setAuth', d, (r) => {
                if (!r) {
                    alert('Failed to set auth.');
                }
                else {
                    setTimeout(() => {
                        $.ajax({
                            url: '/',
                            type: 'GET',
                            username: username,
                            password: newPassword,
                            success: () => {
                                this.close();
                                $.bootstrapGrowl('Auth updated.');
                            }
                        });
                    }, 1000);
                }
            });
        }
    }
}