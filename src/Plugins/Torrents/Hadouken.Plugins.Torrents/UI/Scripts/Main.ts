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
            console.log('torrents plugin loading');

            this._torrentEngine.load();
            this.setupMainMenu();

            this._pageManager.addPage(new Hadouken.Plugins.Torrents.UI.TorrentsListPage(this._eventListener));
        }

        private setupMainMenu(): void {
            var anchor = $('<li><a href="#/torrents">Torrents</a></li>');
            $('#main-menu').append(anchor);
        }
    }
}

try {
    var eventListener = new Hadouken.Events.EventListener();
    var pageManager = Hadouken.UI.PageManager.getInstance();

    eventListener.addHandler('web.signalR.connected', () => {
        var plugin = new Hadouken.Plugins.Torrents.TorrentsPlugin(eventListener, pageManager);
        plugin.load();
    });

    eventListener.connect();
} catch(e) {
    console.log(e);
}