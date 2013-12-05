define(['jquery', 'rpcClient', 'wizardStep'], function($, RpcClient, WizardStep) {
    function ConfigureStep() {
        WizardStep.call(this, 'Core', '/wizards/configure-core.html');
        this.rpc = new RpcClient();
    }

    ConfigureStep.prototype = new WizardStep();
    ConfigureStep.prototype.constructor = ConfigureStep;

    ConfigureStep.prototype.save = function(callback) {
        var cfg = {            
            'plugins.repositoryUrl': this.content.find('#plugins-repositoryUrl').val(),
            'plugins.enableUpdateChecking': this.content.find('#plugins-enableUpdateChecking').is(':checked')
        };

        var username = this.content.find('#auth-username').val();
        var password = this.content.find('#auth-password').val();
        var authData = [username, password, ''];
        
        var that = this;
        that.rpc.callParams('config.setMany', cfg, function() {
            that.rpc.callParams('core.setAuth', authData, function(result) {
                if (!result) {
                    callback();
                } else {
                    $.ajax({
                        url: '/',
                        type: 'GET',
                        username: username,
                        password: password,
                        success: function () {
                            callback();
                        }
                    });
                }
            });
        });
    };

    return ConfigureStep;
});