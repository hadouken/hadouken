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

                this._rpcClient.call('plugins.list', (plugins) => {
                    for (var i = 0; i < plugins.length; i++) {
                        var plugin = plugins[i];
                        var row = template({ plugin: plugin });

                        $('#tbody-plugin-list').append($(row));
                    }
                });
            });
        }
    }
}