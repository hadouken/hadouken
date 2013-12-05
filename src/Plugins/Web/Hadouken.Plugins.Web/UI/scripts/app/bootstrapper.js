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

                    overlay.hide();
                    wizard.show();
                });
            });
        };

        return Bootstrapper;
});