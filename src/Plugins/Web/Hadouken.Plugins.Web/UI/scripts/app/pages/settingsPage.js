define(['rpcClient', 'pluginEngine', 'page', 'handlebars', 'dialogs/changeAuthDialog', 'dialogs/uploadPluginDialog'], function(RpcClient, PluginEngine, Page, Handlebars, ChangeAuthDialog, UploadPluginDialog) {
    function SettingsPage() {
        Page.call(this, '/settings.html', '/settings');
        
        this.rpc = new RpcClient();
        this.pluginEngine = PluginEngine.getInstance();
    }
    
    Page.derive(SettingsPage);

    SettingsPage.prototype.load = function () {
        this.loadPlugins(this.pluginEngine.plugins);

        var that = this;

        $('#btn-change-auth').on('click', function(e) {
            e.preventDefault();

            var dlg = new ChangeAuthDialog();
            dlg.show();
        });

        $('#btn-upload-plugin').on('click', function(e) {
            e.preventDefault();

            var dlg = new UploadPluginDialog();
            dlg.show();
        });

        $('#tbody-plugin-list .btn-configure-plugin').on('click', function(e) {
            var id = $(this).closest('tr').attr('data-plugin-id');
            var plugin = that.pluginEngine.plugins[id];
            
            if (typeof plugin.instance !== 'undefined' && typeof plugin.instance.configure === 'function') {
                plugin.instance.configure();
            }
        });
    };

    SettingsPage.prototype.loadPlugins = function (plugins) {
        if (typeof plugins === 'undefined' || plugins == null)
            return;
        
        // Find container and compile Handlebars template
        var pluginContainer = this.content.find('#tbody-plugin-list');
        var templateHtml = this.content.find('#tmpl-plugin-list-item').html();
        var template = Handlebars.compile(templateHtml);

        var keys = Object.keys(plugins);

        for (var i = 0; i < keys.length; i++) {
            var plugin = plugins[keys[i]];
            var html = template({ plugin: plugin });

            pluginContainer.append(html);
        }
    };

    SettingsPage.prototype.unload = function() {
    };
    
    return SettingsPage;
});