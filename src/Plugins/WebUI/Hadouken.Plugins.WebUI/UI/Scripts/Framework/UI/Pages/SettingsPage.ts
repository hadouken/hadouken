///<reference path="../Page.ts"/>
///<reference path="../../Http/JsonRpcClient.ts"/>
///<reference path="../../Plugins/Plugin.ts"/>
///<reference path="../../Plugins/PluginEngine.ts"/>

module Hadouken.UI.Pages {
    export class SettingsPage extends Page {
        private _pluginEngine: Hadouken.Plugins.PluginEngine = Hadouken.Plugins.PluginEngine.getInstance();
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');
        private _sections: { [id: string]: { (): void; }; } = {};

        constructor() {
            super('/settings.html', [
                '/settings/:section:'
            ]);

            this._sections['general'] = () => this.loadGeneral();
            this._sections['plugins'] = () => this.loadPlugins();
        }

        load(...args: any[]): void {
            if (typeof args[0] !== "undefined" && typeof this._sections[args[0]] !== "undefined") {
                $('#settings-sidebar-menu > li').removeClass('active');
                $('#settings-menu-' + args[0]).parent().addClass('active');

                var sectionCallback = this._sections[args[0]];
                sectionCallback();
            }
        }

        loadGeneral(): void {
            console.log('general page should load');
        }

        loadPlugins(): void {
            $.get('/settings-plugins.html', (html) => {
                $('#settings-section-container').empty();
                $('#settings-section-container').append($(html));

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

                        var pluginId = $(this).attr('data-plugin');
                        that._pluginEngine.plugins[pluginId].instance.configure();
                    });
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