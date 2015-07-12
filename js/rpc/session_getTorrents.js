var session = require("bittorrent").session;

function ignore(torrent, filter) {
    if(filter.tags && filter.tags.length > 0) {
        // If the filter has the 'tags' key, check that
        // the torrent is tagged with all tags in that
        // key.

        var torrentTags = torrent.metadata("tags") || [];

        // Loop through each of the filter tags. If the
        // torrent does not have it, return true.

        for (var i = 0; i < filter.tags.length; i++) {
            if(torrentTags.indexOf(filter.tags[i]) < 0) {
                // The torrent does not have this tag. Ignore it.
                return true;
            }
        };
    }

    // By default, do not ignore any torrent.
    return false;
}

exports.rpc = {
    name: "session.getTorrents",
    method: function(filter) {
        var torrents = session.getTorrents();
        var result   = {};

        // The filter is an object which can contain various properties
        // to match a torrent against. Right now it is only used for tags.
        filter = filter || {};

        for(var i = 0; i < torrents.length; i++) {
            var torrent = torrents[i];

            if(ignore(torrent, filter)) {
                continue;
            }

            var status  = torrent.getStatus();
            var info    = torrent.getTorrentInfo();
            
            var totalSize = -1;
            if(info) totalSize = info.totalSize;

            result[torrent.infoHash] = {
                name:                 status.name,
                infoHash:             torrent.infoHash,
                error:                status.error,
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
                tags:                 torrent.metadata("tags"),
                ratio:                status.ratio,
                eta:                  status.eta
            };
        }

        return result;
    }
};
