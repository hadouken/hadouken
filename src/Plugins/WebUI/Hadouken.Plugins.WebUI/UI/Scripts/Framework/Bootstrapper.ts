///<reference path="Events/EventListener.ts"/>
///<reference path="Plugins/PluginEngine.ts"/>

module Hadouken {
    export class Bootstrapper {


        init(eventListener: Hadouken.Events.EventListener, pluginEngine: Hadouken.Plugins.PluginEngine) {
            console.log("init");

            eventListener.addHandler("web.signalR.connected", (data: any) => {
                pluginEngine.load(() => {
                    console.log("plugins loaded.");
                });
            });

            eventListener.addHandler("web.signalR.fail", (data: any) => {
                // Could not connect to the SignalR server. Nothing we can do.
                // Show failure screen.
            });

            eventListener.connect();
        }
    }
}