///<reference path="../Page.ts"/>
///<reference path="../../Http/JsonRpcClient.ts"/>

module Hadouken.UI.Pages {
    export class SettingsPage extends Page {
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

                var multiCall = {
                    'plugins.list': null,
                    'config.getMany': [ [ 'plugins.repositoryUrl', 'plugins.enableUpdateChecking' ] ]
                };

                this._rpcClient.callParams('core.multiCall', multiCall, (response) => {
                    var plugins = response['plugins.list'];
                    var repositoryUrl = response['config.getMany']['plugins.repositoryUrl'];
                    var enableUpdateCheckingVal = response['config.getMany']['plugins.enableUpdateChecking'];
                    var enableUpdateChecking = false;

                    if (enableUpdateCheckingVal !== null)
                        enableUpdateChecking = enableUpdateCheckingVal;

                    $('#repositoryUrl').val(repositoryUrl);
                    $('#enableUpdateChecking').attr('checked', enableUpdateChecking);

                    $('#btn-save-plugin-settings').on('click', (e) => {
                        e.preventDefault();
                        this.savePluginSettings();
                    });

                    for (var i = 0; i < plugins.length; i++) {
                        var plugin = plugins[i];
                        var row = template({ plugin: plugin });

                        $('#tbody-plugin-list').append($(row));
                    }
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