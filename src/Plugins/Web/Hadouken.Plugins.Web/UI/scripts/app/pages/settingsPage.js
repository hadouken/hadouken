define(['rpcClient', 'page'], function(RpcClient, Page) {
    function SettingsPage() {
        Page.call(this, '/settings.html', '/settings');
        this.rpc = new RpcClient();
    }

    SettingsPage.prototype = new Page();
    SettingsPage.prototype.constructor = SettingsPage;

    SettingsPage.prototype.load = function() {
        this.rpc.call('plugins.list', function(plugins) {
            console.log(plugins);
        });
    };

    SettingsPage.prototype.unload = function() {
        console.log('unloading settings page');
    };

    return SettingsPage;
});