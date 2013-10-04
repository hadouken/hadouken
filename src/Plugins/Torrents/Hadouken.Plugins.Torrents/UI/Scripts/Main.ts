///<reference path="hdkn.d.ts"/>
///<reference path="BitTorrent/BitTorrentEngine.ts"/>
///<reference path="UI/TorrentsListPage.ts"/>

module Hadouken.Plugins.Torrents {
    export class TorrentsPlugin {
        private _eventListener: Hadouken.Events.EventListener;
        private _torrentEngine: Hadouken.Plugins.Torrents.BitTorrent.BitTorrentEngine;

        constructor(eventListener: Hadouken.Events.EventListener) {
            this._eventListener = eventListener;
            this._torrentEngine = new Hadouken.Plugins.Torrents.BitTorrent.BitTorrentEngine(eventListener);
        }

        load(): void {
            console.log('torrents plugin loading');

            this._torrentEngine.load();
            this.setupMainMenu();

            return;
        }

        private setupMainMenu(): void {
            var anchor = $('<li><a href="#">Torrents</a></li>');
            anchor.find('a').on('click', (e) => {
                e.preventDefault();
                new Hadouken.Plugins.Torrents.UI.TorrentsListPage(this._eventListener).load();
            });

            $('#main-menu').append(anchor);
        }
    }
}

try {
    var eventListener = new Hadouken.Events.EventListener();

    eventListener.addHandler('web.signalR.connected', () => {
        var plugin = new Hadouken.Plugins.Torrents.TorrentsPlugin(eventListener);
        plugin.load();
    });

    eventListener.connect();
} catch(e) {
    console.log(e);
}