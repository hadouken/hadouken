///<reference path="../Http/JsonRpcClient.ts"/>

module Hadouken.Plugins {
    class PluginDto {
        constructor(public name: string, public version: string, public state: string) { }
    }

    export class PluginEngine {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient("/jsonrpc");

        load(): void {
            // Get all plugins
            this._rpcClient.call("plugins.list", (plugins: PluginDto[]) => {
                for (var i = 0; i < plugins.length; i++) {
                    var plugin = plugins[i];
                    console.log(plugin.name + " (" + plugin.version + ")");
                }
            });
        }
    }
}