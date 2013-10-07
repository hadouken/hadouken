///<reference path="../Page.ts"/>

module Hadouken.UI.Pages {
    export class SettingsPage extends Page {
        constructor() {
            super('/settings.html', [
                '/settings'
            ]);
        }
    }
}