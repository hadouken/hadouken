var session = require("bittorrent").session;

exports.rpc = {
    name: "webui.list",
    method: function() {
        var torrents = session.getTorrents();
        var torrentsList = [];
        var labels = {};

        for(var i = 0; i < torrents.length; i++) {
            var torrent = torrents[i];
            var status  = torrent.getStatus();
            var info    = torrent.getTorrentInfo();

            var totalSize = -1;
            if(info) totalSize = info.totalSize;

            var state = 0;
            if(!status.isPaused) { state = (state | 1); } // started
            if(status.state === 0
                || status.state === 1
                || status.state === 7) { state = (state | 2); } // checking
            if(status.error) { state = (state | 16); } // error
            if(!status.autoManaged && status.isPaused) { state = (state | 32); } // paused
            if(status.autoManaged && status.isPaused) { state = (state | 64); } // queued

            var label = torrent.metadata("label");

            torrentsList.push([
                torrent.infoHash,
                state,
                status.name,
                totalSize,
                (status.progress * 1000) | 0,
                status.downloadedBytesTotal,
                status.uploadedBytesTotal,
                (status.ratio * 1000) | 0,
                status.downloadRate,
                status.uploadRate,
                status.eta,
                label || "",
                status.numPeers - status.numSeeds,
                status.listPeers,
                status.numSeeds,
                status.listSeeds,
                status.distributedCopies,
                status.queuePosition,
                (totalSize > 0 ? totalSize - status.wantedBytes : -1),
                "", // download url
                "", // rss feed url,
                status.error || "",
                "", // stream id
                status.addedTime,
                status.completedTime,
                "", // app update url
                status.savePath
            ]);

            if(label) {
                if(!labels[label]) {
                    labels[label] = 1;
                } else {
                    labels[label] += 1;
                }
            }
        }

        var labelList = [];

        for(var key in labels) {
            labelList.push([ key, labels[key] ]);
        }

        return {
            label: labelList,
            torrents: torrentsList
        };
    }
};
