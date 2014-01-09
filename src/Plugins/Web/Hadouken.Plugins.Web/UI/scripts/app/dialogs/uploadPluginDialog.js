define(['dialog', 'rpcClient', 'overlay'], function(Dialog, RpcClient, Overlay) {
    function UploadPluginDialog() {
        Dialog.call(this, '/dialogs/upload-plugin.html');
        this.rpc = new RpcClient();
    }

    UploadPluginDialog.prototype = new Dialog();
    UploadPluginDialog.prototype.constructor = UploadPluginDialog;

    UploadPluginDialog.prototype.load = function (callback) {
        callback();
    };

    UploadPluginDialog.prototype.save = function () {
    };

    return UploadPluginDialog;
});