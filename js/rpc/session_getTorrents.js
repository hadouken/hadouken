var session = require("bittorrent").session;

exports.rpc = {
    name: "session.getTorrents",
    method: function() {
        var torrents = session.getTorrents();
        var result   = {};

        for(var i = 0; i < torrents.length; i++) {
            var torrent = torrents[i];
            var status  = torrent.getStatus();
            var info    = torrent.getTorrentInfo();

            var totalSize = -1;
            if(info) totalSize = info.totalSize;

            result[torrent.infoHash] = {
                name:                 status.name,
                infoHash:             torrent.infoHash,
                progress:             status.progress,
                savePath:             status.savePath,
                downloadRate:         status.downloadRate,
                uploadRate:           status.uploadRate,
                downloadedBytes:      status.downloadedBytes,
                downloadedBytesTotal: status.downloadedBytesTotal,
                uploadedBytes:        status.uploadedBytes,
                uploadedBytesTotal:   status.uploadedBytesTotal,
                numPeers:             status.numPeers,
                numSeeds:             status.numSeeds,
                totalSize:            totalSize,
                state:                status.state,
                isFinished:           status.isFinished,
                isMovingStorage:      status.isMovingStorage,
                isPaused:             status.isPaused,
                isSeeding:            status.isSeeding,
                isSequentialDownload: status.isSequentialDownload,
                queuePosition:        torrent.queuePosition,
                tags:                 torrent.tags
            };
        }

        return result;
    }
};
