///<reference path="Events/EventListener.ts"/>
///<reference path="Plugins/PluginEngine.ts"/>

module Hadouken {
    export class Bootstrapper {


        init(eventListener: Hadouken.Events.EventListener, pluginEngine: Hadouken.Plugins.PluginEngine) {
            console.log("init");

            eventListener.addHandler("web.signalR.connected", () => {
                pluginEngine.load(() => {
                    console.log("plugins loaded.");
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