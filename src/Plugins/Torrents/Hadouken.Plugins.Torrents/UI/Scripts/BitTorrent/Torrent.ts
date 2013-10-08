module Hadouken.Plugins.Torrents.BitTorrent {
    export class Torrent {
        id: string;
        name: string;
        size: number;
        state: string;
        label: string;
        progress: number;

        files: Array<TorrentFile>;
    }

    export class TorrentFile {
        index: number;
        bitField: Array<number>;
        bitFieldPercentComplete: number;
        bytesDownloaded: number;
        endPieceIndex: number;
        fullPath: string;
        length: number;
        path: string;
        priority: string;
        startPieceIndex: number;
    }
}