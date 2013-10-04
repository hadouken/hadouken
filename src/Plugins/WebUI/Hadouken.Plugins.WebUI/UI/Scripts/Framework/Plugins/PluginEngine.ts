///<reference path="../Http/JsonRpcClient.ts"/>

module Hadouken.Plugins {
    class PluginDto {
        constructor(public name: string, public version: string, public state: string) { }
    }

    export class PluginEngine {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient("/jsonrpc");
        private _plugins: PluginDto[] = new Array<PluginDto>();

        load(callback: { (): void; }): void {
            this._rpcClient.call("plugins.list", (plugins: PluginDto[]) => {
                for (var i = 0; i < plugins.length; i++) {
                    var plugin = plugins[i];
                    this._plugins.push(plugin);
                }

                this.loadPlugins(callback);
            });
        }

        private loadPlugins(callback: { (): void; }): void {
            var requestedFiles = 0;
            var requestCallback = () => {
                requestedFiles++;

                if (requestedFiles == this._plugins.length) {
                    callback();
                }
            };

            for (var i = 0; i < this._plugins.length; i++) {
                var plugin = this._plugins[i];

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