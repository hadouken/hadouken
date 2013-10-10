///<reference path="hdkn.d.ts"/>
///<reference path="BitTorrent/BitTorrentEngine.ts"/>
///<reference path="UI/TorrentDetailsPage.ts"/>
///<reference path="UI/TorrentsListPage.ts"/>
///<reference path="UI/ConfigureDialog.ts"/>

module Hadouken.Plugins.Torrents {
    export class TorrentsPlugin extends Hadouken.Plugins.Plugin {
        private _rpcClient: Hadouken.Http.JsonRpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');
        private _eventListener: Hadouken.Events.EventListener = new Hadouken.Events.EventListener();
        private _pageManager: Hadouken.UI.PageManager;
        private _torrentEngine: Hadouken.Plugins.Torrents.BitTorrent.BitTorrentEngine;

        constructor() {
            super();
            this._pageManager = Hadouken.UI.PageManager.getInstance();
        }

        load(): void {
            this.setupNotifications();
            this.setupMainMenu();
            this.loadPages();
        }

        loadFirstTimeSetup(container: any): void {
            $.get('/plugins/core.torrents/forms/first-time-setup.html', (h) => {
                container.append(h);
            });
        }

        saveFirstTimeSetup(container: any, callback: { (): void; }): void {
            var savePath = container.find('#torrents-savePath').val();
            this._rpcClient.callParams('config.set', ['bt.downloads.savePath', savePath], (e) => {
                callback();
            });
        }

        unload(): void { }

        configure(): void {
            new Hadouken.Plugins.Torrents.UI.ConfigureDialog().show();
        }

        private setupNotifications(): void {
            this._eventListener.addHandler('torrent.completed', (torrent) => {
                $.bootstrapGrowl(torrent.name + ' completed!');
            });

            this._eventListener.connect();
        }

        private setupMainMenu(): void {
            var anchor = $('<li><a href="#/torrents"><i class="icon-tasks"></i> Torrents</a></li>');
            $('#main-menu').append(anchor);
        }

        private loadPages(): void {
            this._pageManager.addPage(new Hadouken.Plugins.Torrents.UI.TorrentsListPage());
            this._pageManager.addPage(new Hadouken.Plugins.Torrents.UI.TorrentDetailsPage());
        }
    }
}

try {
    var plugin = new Hadouken.Plugins.Torrents.TorrentsPlugin();
    plugin.load();

    var pluginEngine = Hadouken.Plugins.PluginEngine.getInstance();
    pluginEngine.setPlugin('core.torrents', plugin);
} catch(e) {
    console.log(e);
}