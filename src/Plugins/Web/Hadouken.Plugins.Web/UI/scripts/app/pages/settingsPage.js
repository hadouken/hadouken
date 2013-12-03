define(['rpcClient', 'page', 'handlebars'], function(RpcClient, Page, Handlebars) {
    function SettingsPage() {
        Page.call(this, '/settings.html', '/settings');
        this.rpc = new RpcClient();
    }

    SettingsPage.prototype = new Page();
    SettingsPage.prototype.constructor = SettingsPage;

    SettingsPage.prototype.load = function () {
        var that = this;
        
        this.rpc.call('plugins.list', function(plugins) {
            that.loadPlugins(plugins);
        });
    };

    SettingsPage.prototype.loadPlugins = function (plugins) {
        if (typeof plugins === 'undefined' || plugins == null)
            return;
        
        var pluginContainer = this.content.find('#tbody-plugin-list');
        var templateHtml = this.content.find('#tmpl-plugin-list-item').html();
        var template = Handlebars.compile(templateHtml);

        for (var i = 0; i < plugins.length; i++) {
            var plugin = plugins[i];
            var html = template({ plugin: plugin });

            pluginContainer.append(html);
        }
    };

    SettingsPage.prototype.unload = function() {
        console.log('unloading settings page');
    };

    return SettingsPage;
});