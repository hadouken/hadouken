var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.getTrackers",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);
        
        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        var trackers = torrent.getTrackers();
        var result   = [];

        for(var i = 0; i < trackers.length; i++) {
            var tracker = trackers[i];

            result.push({
                failCount:    tracker.failCount,
                failLimit:    tracker.failLimit,
                isUpdating:   tracker.isUpdating,
                isVerified:   tracker.isVerified,
                lastError:    tracker.lastError,
                message:      tracker.message,
                minAnnounce:  tracker.minAnnounce,
                nextAnnounce: tracker.nextAnnounce,
                scrape: {
                    complete: tracker.scrapeComplete,
                    dowloaded: tracker.scrapeDownloaded,
                    incomplete: tracker.scrapeIncomplete
                },
                source:       tracker.source,
                tier:         tracker.tier,
                url:          tracker.url
            });
        }

        return result;
    }
};
