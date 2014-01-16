define(['dialog', 'rpcClient', 'overlay'], function(Dialog, RpcClient, Overlay) {
    function UploadPluginDialog() {
        Dialog.call(this, '/dialogs/upload-plugin.html');
        this.rpc = new RpcClient();
    }

    UploadPluginDialog.prototype = new Dialog();
    UploadPluginDialog.prototype.constructor = UploadPluginDialog;

    UploadPluginDialog.prototype.load = function (callback) {
        var that = this;
        this.content.find('#btn-upload-plugin').on('click', function(e) {
            e.preventDefault();
            that.upload();
        });

        callback();
    };

    UploadPluginDialog.prototype.upload = function() {
        this.content.find('#btn-upload-plugin').attr('disabled', true);

        var fileInput = this.content.find('#plugin-package-file')[0];
        var reader = new FileReader();
        var that = this;

        reader.onload = function(e) {
            var data = [
                e.target.result.split(',')[1],
                that.content.find('#current-password').val()
            ];

            that.rpc.callParams('plugins.upload', data, function(response) {
                if (!response) {
                    alert('Upload failed');
                }

                that.content.find('#btn-upload-plugin').attr('disabled', false);
            });
        };

        reader.readAsDataURL(fileInput.files[0]);
    };

    return UploadPluginDialog;
});