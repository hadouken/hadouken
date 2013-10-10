///<reference path="../Dialog.ts"/>
///<reference path="../../Plugins/PluginEngine.ts"/>
///<reference path="../../Http/JsonRpcClient.ts"/>

module Hadouken.UI.Dialogs {
    export class FirstTimeSetupDialog extends Hadouken.UI.Dialog {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');
        private _pluginEngine: Hadouken.Plugins.PluginEngine = Hadouken.Plugins.PluginEngine.getInstance();

        constructor() {
            super('/dialogs/first-time-setup.html');
        }

        onShow(): void {
            var pluginIds = Object.keys(this._pluginEngine.plugins);

            for (var i = 0; i < pluginIds.length; i++) {
                var plugin = this._pluginEngine.plugins[pluginIds[i]];

                if (!plugin.instance)
                    continue;

                var container = $('<div class="setup-section"></div>');
                container.attr('data-plugin-id', plugin.name);

                this.getContent().find('.modal-body').append(container);

                plugin.instance.loadFirstTimeSetup(container);
            }

            this.getContent().find('#btn-save-setup').on('click', (e) => this.save());
        }

        save(): void {
            this.getContent().find('.overlay').removeClass('hide');
            this.getContent().find('#btn-save-setup').attr('disabled', true);

            var cfg = {
                'plugins.repositoryUrl': this.getContent().find('#plugins-repositoryUrl').val(),
                'plugins.enableUpdateChecking': this.getContent().find('#plugins-enableUpdateChecking').is(':checked'),
                'web.firstTimeSetupShown': true
            };

            this._rpcClient.callParams('config.setMany', cfg, (r) => {
                if (r) {
                    this.savePlugins();
                }
            });
        }

        savePlugins(): void {
            var pluginSections = this.getContent().find('.setup-section[data-plugin-id]');
            var pluginsToSave = pluginSections.length;
            var pluginsSaved = 0;

            var callback = () => {
                pluginsSaved += 1;

                if (pluginsSaved == pluginsToSave) {
                    this.saveAuth();
                }
            };

            for (var i = 0; i < pluginSections.length; i++) {
                var section = $(pluginSections[i]);
                var pluginId = section.attr('data-plugin-id');
                var plugin = this._pluginEngine.plugins[pluginId];

                if (!plugin.instance)
                    continue;

                plugin.instance.saveFirstTimeSetup(section, callback);
            }
        }

        saveAuth(): void {
            var usr = this.getContent().find('#auth-username').val();
            var pwd = this.getContent().find('#auth-password').val();

            this._rpcClient.callParams('core.setAuth', [usr, pwd, ''], (r) => {
                if (r) {
                    setTimeout(() => {
                        $.ajax({
                            url: '/',
                            type: 'GET',
                            username: usr,
                            password: pwd,
                            success: () => {
                                this.close();
                                $.bootstrapGrowl('Hadouken configured.');
                            }
                        });
                    }, 1000);
                }
            });
        }

        onClosed() {
            this._rpcClient.callParams('config.set', [ 'web.firstTimeSetupShown', true ], (c) => { });
        }
    }
}