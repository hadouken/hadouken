module Hadouken.Plugins.Torrents.BitTorrent {
    export class Torrent {
        id: string;
        name: string;
        size: number;
        state: string;
        label: string;
        progress: number;
    }
}