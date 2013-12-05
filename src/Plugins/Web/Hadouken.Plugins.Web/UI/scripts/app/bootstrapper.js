define(
    [
        'jquery',
        'bootstrap',
        'overlay',
        'eventListener',
        'pageManager',
        'pluginEngine',
        'pages/settingsPage',
        'wizard',
        'wizardSteps/configureStep'
    ],
    function ($, $bs, Overlay, EventListener, PageManager, PluginEngine, SettingsPage, Wizard, ConfigureStep) {
        function Bootstrapper() {
            this.eventListener = new EventListener();
        }

        Bootstrapper.prototype.init = function () {
            var overlay = new Overlay('body');
            overlay.show();

            this.eventListener.connect(function() {
                var pageManager = PageManager.getInstance();
                pageManager.addPage(new SettingsPage());

                var pluginEngine = PluginEngine.getInstance();
                pluginEngine.load(function () {
                    pageManager.init();

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
            });
        };

        return Bootstrapper;
});