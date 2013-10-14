///<reference path="../WizardStep.ts"/>
///<reference path="../../Http/JsonRpcClient.ts"/>

module Hadouken.UI.WizardSteps {
    export class ConfigureCoreStep extends Hadouken.UI.WizardStep {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');

        constructor() {
            super('/wizards/configure-core.html', 'Core');
        }

        public loadData(callback: any): void {
            console.log('loading core configure data');

            var greeter = this.content.find('h2.greeter .name');
            this.content.find('#auth-username').on('blur', function (e) {
                var val = $(this).val();

                if (val === '')
                    val = 'user';

                greeter.fadeOut('fast', () => {
                    greeter.text(val).fadeIn('fast');
                });
            });

            callback();
        }

        public saveData(callback: any): void {
            var cfg = {
                'plugins.repositoryUrl': this.content.find('#plugins-repositoryUrl').val(),
                'plugins.enableUpdateChecking': this.content.find('#plugins-enableUpdateChecking').is(':checked'),
                'core.isConfigured': true
            };

            this._rpcClient.callParams('config.setMany', cfg, () => {
                callback();
            });
        }
    }
}