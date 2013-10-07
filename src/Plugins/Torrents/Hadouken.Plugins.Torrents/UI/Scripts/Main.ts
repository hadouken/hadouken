///<reference path="hdkn.d.ts"/>
///<reference path="BitTorrent/BitTorrentEngine.ts"/>
///<reference path="UI/TorrentsListPage.ts"/>

module Hadouken.Plugins.Torrents {
    export class TorrentsPlugin {
        private _eventListener: Hadouken.Events.EventListener;
        private _pageManager: Hadouken.UI.PageManager;
        private _torrentEngine: Hadouken.Plugins.Torrents.BitTorrent.BitTorrentEngine;

        constructor(eventListener: Hadouken.Events.EventListener, pageManager: Hadouken.UI.PageManager) {
            this._eventListener = eventListener;
            this._pageManager = pageManager;
            this._torrentEngine = new Hadouken.Plugins.Torrents.BitTorrent.BitTorrentEngine(eventListener);
        }

        load(): void {
            this.setupMainMenu();
            this.setupPageManager();
            this.setupTorrentEngine();

            this._eventListener.connect();
        }

        private setupMainMenu(): void {
            var anchor = $('<li><a href="#/torrents">Torrents</a></li>');
            $('#main-menu').append(anchor);
        }

        private setupPageManager(): void {
            this._pageManager.addPage(new Hadouken.Plugins.Torrents.UI.TorrentsListPage(this._eventListener));
        }

        private setupTorrentEngine(): void {
            this._torrentEngine.load();
        }
    }
}

try {
    var eventListener = new Hadouken.Events.EventListener();
    var pageManager = Hadouken.UI.PageManager.getInstance();

    var plugin = new Hadouken.Plugins.Torrents.TorrentsPlugin(eventListener, pageManager);
    plugin.load();
} catch(e) {
    console.log(e);
}