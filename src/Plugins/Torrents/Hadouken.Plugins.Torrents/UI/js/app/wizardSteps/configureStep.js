define(['rpcClient', 'wizardStep'], function(RpcClient, WizardStep) {
    function ConfigureStep() {
        WizardStep.call(this, 'Torrents', '/plugins/core.torrents/wizards/first-time-setup.html');
        this.rpc = new RpcClient();
    }

    ConfigureStep.prototype = new WizardStep();
    ConfigureStep.prototype.constructor = ConfigureStep;

    ConfigureStep.prototype.save = function(callback) {
        var data = [
            'bt.downloads.savePath', this.content.find('#torrents-savePath').val()
        ];

        this.rpc.callParams('config.set', data, function() {
            callback();
        });
    };

    return ConfigureStep;
});