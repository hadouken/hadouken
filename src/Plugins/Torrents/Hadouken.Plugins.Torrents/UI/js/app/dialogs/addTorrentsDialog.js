define(['jquery', 'dialog', 'rpcClient'], function($, Dialog, RpcClient) {
    function AddTorrentsDialog() {
        Dialog.call(this, '/plugins/core.torrents/dialogs/add-torrents.html');
        this.rpc = new RpcClient();
    }

    AddTorrentsDialog.prototype = new Dialog();
    AddTorrentsDialog.prototype.constructor = AddTorrentsDialog;

    AddTorrentsDialog.prototype.load = function (callback) {
        var that = this;
        this.content.find('#btn-add-torrents').on('click', function(e) {
            e.preventDefault();
            that.addTorrents();
        });

        callback();
    };

    AddTorrentsDialog.prototype.addTorrents = function () {
        this.content.find('#btn-add-torrents').attr('disabled', true);

        var fileInput = this.content.find('#torrent-files')[0];
        var reader = new FileReader();
        var filesAdded = 0;
        var that = this;
        
        reader.onload = function(e) {
            var data = [
                e.target.result.split(',')[1],
                '',
                ''
            ];

            that.rpc.callParams('torrents.addFile', data, function() {
                filesAdded += 1;

                if (filesAdded == fileInput.files.length) {
                    that.close();
                }
            });
        };
        
        for (var i = 0; i < fileInput.files.length; i++) {
            reader.readAsDataURL(fileInput.files[i]);
        }
    };

    return AddTorrentsDialog;
});