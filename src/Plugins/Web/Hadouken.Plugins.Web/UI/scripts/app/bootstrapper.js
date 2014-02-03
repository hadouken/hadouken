define(
    [
        'jquery',
        'bootstrap',
        'handlebars',
        'overlay',
        'eventListener',
        'pageManager',
        'pluginEngine',
        'pages/settingsPage',
        'rpcClient',
        'wizard',
        'wizardSteps/configureStep'
    ],
    function ($, $bs, Handlebars, Overlay, EventListener, PageManager, PluginEngine, SettingsPage, RpcClient, Wizard, ConfigureStep) {
        function Bootstrapper() {
            this.eventListener = new EventListener();
        }

        Bootstrapper.prototype.init = function () {
            // Register Handlebars helpers
            Handlebars.registerHelper('fileSize', function(size) {
                if (size < 0)
                    return 'NaN';

                var i = Math.floor(Math.log(size) / Math.log(1024));
                return (size / Math.pow(1024, i)).toFixed(2) + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
            });

            var overlay = new Overlay('body');
            overlay.show();

            var that = this;
            this.eventListener.subscribe('sys.unloading', function() { that.shutdown(); });

            this.eventListener.connect(function() {
                var pageManager = PageManager.getInstance();
                pageManager.addPage(new SettingsPage());

                var pluginEngine = PluginEngine.getInstance();
                pluginEngine.load(function () {
                    pageManager.init();

                    // Only show the first time wizard if we haven't set 'ui.configured' to true.
                    var rpc = new RpcClient();
                    rpc.callParams('config.get', 'ui.configured', function(configured) {
                        if (!configured) {
                            rpc.callParams('config.set', ['ui.configured', true], function() {
                                // Show wizard
                                var wizard = new Wizard('First time setup');
                                wizard.steps.push(new ConfigureStep());

                                // Add steps from plugins
                                var plugins = Object.keys(pluginEngine.plugins);
                                for (var i = 0; i < plugins.length; i++) {
                                    var plugin = pluginEngine.plugins[plugins[i]];

                                    if (!plugin.instance) {
                                        continue;
                                    }

                                    var step = plugin.instance.configure('wizard');

                                    if (!step) {
                                        continue;
                                    }

                                    wizard.steps.push(step);
                                }

                                overlay.hide();
                                wizard.show();
                            });
                        } else {
                            overlay.hide();
                        }
                    });
                });
            });
        };

        Bootstrapper.prototype.shutdown = function() {
            // Get the page manager and unload current page
            var pageManager = PageManager.getInstance();
            if (pageManager.currentPage != null) {
                pageManager.currentPage.unload();
            }

            // Get the plugin engine and unload all plugins
            var pluginEngine = PluginEngine.getInstance();
            var pluginKeys = Object.keys(pluginEngine.plugins);

            for (var i = 0; i < pluginKeys.length; i++) {
                var plugin = pluginEngine.plugins[pluginKeys[i]];
                var instance = plugin.instance;

                if (!instance) {
                    continue;
                }

                if (typeof instance.unload === 'function') {
                    instance.unload();
                }
            }

            $('#page-container').empty();
        };

        return Bootstrapper;
});