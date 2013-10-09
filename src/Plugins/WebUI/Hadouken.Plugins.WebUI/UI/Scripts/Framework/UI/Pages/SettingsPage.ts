///<reference path="../Page.ts"/>
///<reference path="../Dialogs/ChangeAuthDialog.ts"/>
///<reference path="../../Http/JsonRpcClient.ts"/>
///<reference path="../../Plugins/Plugin.ts"/>
///<reference path="../../Plugins/PluginEngine.ts"/>

module Hadouken.UI.Pages {
    export class SettingsPage extends Page {
        private _pluginEngine: Hadouken.Plugins.PluginEngine = Hadouken.Plugins.PluginEngine.getInstance();
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');

        constructor() {
            super('/settings.html', [
                '/settings/:section:'
            ]);
        }

        load(...args: any[]): void {
            this.loadGeneral();
            this.loadPlugins();
        }

        loadGeneral(): void {
            $('#btn-change-auth').on('click', (e) => {
                e.preventDefault();
                new Hadouken.UI.Dialogs.ChangeAuthDialog().show();
            });
        }

        loadPlugins(): void {
            var template = Handlebars.compile($('#tmpl-plugin-list-item').html());

            var data = [['plugins.repositoryUrl', 'plugins.enableUpdateChecking']];

            this._rpcClient.callParams('config.getMany', data, (response) => {
                var repositoryUrl = response['plugins.repositoryUrl'];
                var enableUpdateCheckingVal = response['plugins.enableUpdateChecking'];
                var enableUpdateChecking = false;

                if (enableUpdateCheckingVal !== null)
                    enableUpdateChecking = enableUpdateCheckingVal;

                $('#repositoryUrl').val(repositoryUrl);
                $('#enableUpdateChecking').attr('checked', enableUpdateChecking);

                $('#btn-save-plugin-settings').on('click', (e) => {
                    e.preventDefault();
                    this.savePluginSettings();
                });

                var pluginIds = Object.keys(this._pluginEngine.plugins);

                for (var i = 0; i < pluginIds.length; i++) {
                    var key = pluginIds[i];
                    var plugin = this._pluginEngine.plugins[key];

                    var row = template({ plugin: plugin });
                    $('#tbody-plugin-list').append($(row));
                }

                var that = this;

                $('.btn-configure-plugin').on('click', function (e) {
                    e.preventDefault();

                    var pluginId = $(this).closest("tr").attr('data-plugin-id');
                    that._pluginEngine.plugins[pluginId].instance.configure();
                });
            });
        }

        savePluginSettings(): void {
            var repositoryUrl = $('#repositoryUrl').val();
            var enableUpdateChecking = $('#enableUpdateChecking').is(':checked');

            var d = {
                'plugins.repositoryUrl': repositoryUrl,
                'plugins.enableUpdateChecking': enableUpdateChecking
            };

            this._rpcClient.callParams('config.setMany', d, (result) => {
                //
            });
        }
    }
}