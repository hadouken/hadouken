define(['jquery', 'dialog', 'rpcClient'], function($, Dialog, RpcClient) {
    function AddTorrentsDialog() {
        Dialog.call(this, '/plugins/core.torrents/dialogs/add-torrents.html');
        this.rpc = new RpcClient();
    }

    AddTorrentsDialog.prototype = new Dialog();
    AddTorrentsDialog.prototype.constructor = AddTorrentsDialog;

    AddTorrentsDialog.prototype.load = function (callback) {
        callback();
    };

    return AddTorrentsDialog;
});