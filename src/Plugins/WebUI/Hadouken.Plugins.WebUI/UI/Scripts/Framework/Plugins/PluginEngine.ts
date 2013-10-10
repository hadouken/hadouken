///<reference path="../Http/JsonRpcClient.ts"/>
///<reference path="Plugin.ts"/>

module Hadouken.Plugins {
    class PluginDto {
        constructor(public name: string, public version: string, public state: string) { }
    }

    export class PluginManager {
        public instance: Hadouken.Plugins.Plugin = null;

        constructor(public name: string, public version: string, public state: string, public memoryUsage: number) { }
    }

    export class PluginEngine {
        private static _instance: PluginEngine = null;
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient("/jsonrpc");
        public plugins: { [id: string]: Hadouken.Plugins.PluginManager; } = {};

        constructor() {
            if (PluginEngine._instance) {
                throw new Error();
            }

            PluginEngine._instance = this;
        }

        public static getInstance(): PluginEngine {
            if (PluginEngine._instance === null) {
                PluginEngine._instance = new PluginEngine();
            }

            return PluginEngine._instance;
        }

        load(callback: { (): void; }): void {
            this._rpcClient.call("plugins.list", (pluginManagers: PluginManager[]) => {
                for (var i = 0; i < pluginManagers.length; i++) {
                    var pluginManager = pluginManagers[i];
                    this.plugins[pluginManager.name] = pluginManager;
                }

                this.loadPlugins(callback);
            });
        }

        setPlugin(id: string, plugin: Hadouken.Plugins.Plugin): void {
            if (typeof this.plugins[id] === "undefined")
                return;

            this.plugins[id].instance = plugin;
        }

        private loadPlugins(callback: { (): void; }): void {
            var pluginKeys = Object.keys(this.plugins);
            var requestedFiles = 0;
            var requestCallback = () => {
                requestedFiles++;

                if (requestedFiles == pluginKeys.length) {
                    callback();
                }
            };

            for (var i = 0; i < pluginKeys.length; i++) {
                var plugin = this.plugins[pluginKeys[i]];

                if (plugin.name === "core.webui") {
                    requestedFiles++;
                    continue;
                }

                $.getScript('/plugins/' + plugin.name + '/scripts/main.ts')
                    .done(requestCallback)
                    .fail(requestCallback);
            }
        }
    }
}