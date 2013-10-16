///<reference path="Events/EventListener.ts"/>
///<reference path="Plugins/PluginEngine.ts"/>
///<reference path="UI/PageManager.ts"/>
///<reference path="UI/Pages/SettingsPage.ts"/>
///<reference path="UI/Overlay.ts"/>
///<reference path="Http/JsonRpcClient.ts"/>
///<reference path="UI/Wizard.ts"/>
///<reference path="UI/WizardSteps/ConfigureCoreStep.ts"/>

module Hadouken {
    export class Bootstrapper {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');

        init(eventListener: Hadouken.Events.EventListener, pluginEngine: Hadouken.Plugins.PluginEngine, pageManager: Hadouken.UI.PageManager) {
            // Add pages
            pageManager.addPage(new Hadouken.UI.Pages.SettingsPage());

            var overlay = new Hadouken.UI.Overlay('icon-refresh loading');
            overlay.show($(document.body));

            eventListener.addHandler("web.signalR.connected", () => {
                pluginEngine.load(() => {
                    pageManager.init();

                    var wiz = new Hadouken.UI.Wizard('First time setup wizard');
                    wiz.addStep(new Hadouken.UI.WizardSteps.ConfigureCoreStep());

                    var pluginKeys = Object.keys(pluginEngine.plugins);

                    for (var i = 0; i < pluginKeys.length; i++) {
                        var key = pluginKeys[i];
                        var plugin = pluginEngine.plugins[key];

                        if (!plugin.instance)
                            continue;

                        var step = plugin.instance.initialConfiguration();

                        if (!step)
                            continue;

                        wiz.addStep(step);
                    }
                    overlay.hide();
                    wiz.show();
                });
            });

            eventListener.addHandler("web.signalR.disconnected", () => {
                // Could not connect to the SignalR server. Nothing we can do.
                // Show failure screen.
                console.error('connection failure');
            });

            eventListener.connect();
        }
    }
}