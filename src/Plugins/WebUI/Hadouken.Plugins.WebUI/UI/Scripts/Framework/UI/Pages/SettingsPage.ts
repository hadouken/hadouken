///<reference path="../Page.ts"/>

module Hadouken.UI.Pages {
    export class SettingsPage extends Page {
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
            $.get('/settings-plugins.html', function (html) {
                $('#settings-section-container').empty();
                $('#settings-section-container').append($(html));
            });
        }
    }
}