define(['dialog', 'rpcClient', 'overlay'], function(Dialog, RpcClient, Overlay) {
    function ChangeAuthDialog() {
        Dialog.call(this, '/dialogs/change-auth.html');
        this.rpc = new RpcClient();
    }

    ChangeAuthDialog.prototype = new Dialog();
    ChangeAuthDialog.prototype.constructor = ChangeAuthDialog;

    ChangeAuthDialog.prototype.load = function (callback) {
        var that = this;
        
        this.rpc.call('core.getAuthInfo', function(result) {
            that.content.find('#auth-username').text(result.userName).focus();

            that.content.find('#btn-save-auth').on('click', function(e) {
                e.preventDefault();
                that.save();
            });

            callback();
        });
    };

    ChangeAuthDialog.prototype.save = function() {
        var o = new Overlay();
        o.show(this.content.find('.modal-body'));

        var username = this.content.find('#auth-username').val();
        var newPassword = this.content.find('#auth-newPassword').val();
        var currentPassword = this.content.find('#auth-currentPassword').val();

        var data = [username, newPassword, currentPassword];

        var that = this;
        this.rpc.callParams('core.setAuth', data, function(r) {
            if (!r) {
                alert('Failed to set auth.');
            } else {
                setTimeout(function() {
                    $.ajax({
                        url: '/',
                        type: 'GET',
                        username: username,
                        password: newPassword,
                        success: function() {
                            that.close();
                        }
                    });
                }, 1000);
            }
        });
    };

    return ChangeAuthDialog;
});