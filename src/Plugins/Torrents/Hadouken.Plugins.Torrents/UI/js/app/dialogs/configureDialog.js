define(['jquery', 'dialog', 'rpcClient'], function($, Dialog, RpcClient) {
    function ConfigureDialog() {
        Dialog.call(this, '/plugins/core.torrents/dialogs/configure.html');
        this.rpc = new RpcClient();
    }

    ConfigureDialog.prototype = new Dialog();
    ConfigureDialog.prototype.constructor = ConfigureDialog;

    ConfigureDialog.prototype.load = function (callback) {
        var that = this;

        this.content.find('#btn-save-torrent-config').on('click', function(e) {
            e.preventDefault();
            that.save();
        });
        
        this.rpc.callParams('config.getSection', 'bt.', function(result) {
            var keys = Object.keys(result);
            
            for (var i = 0; i < keys.length; i++) {
                var key = keys[i];
                var htmlKey = key.replace(/\./g, '-');

                that.content.find('#' + htmlKey).val(result[key]);
            }

            that.keys = keys;

            callback();
        });
    };

    ConfigureDialog.prototype.save = function() {
        var data = {};
        
        for (var i = 0; i < this.keys.length; i++) {
            var key = this.keys[i];
            var htmlKey = key.replace(/\./g, '-');
            var value = this.content.find('#' + htmlKey).val();

            data[key] = value;
        }

        var that = this;
        this.rpc.callParams('config.setMany', data, function(result) {
            if (result) {
                that.close();
            }
        });
    };

    return ConfigureDialog;
});