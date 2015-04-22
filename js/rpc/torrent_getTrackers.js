var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.getTrackers",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);
        
        if(!torrent) {
            return null;
        }

        var trackers = torrent.getTrackers();
        var result   = [];

        for(var i = 0; i < trackers.length; i++) {
            var tracker = trackers[i];

            result.push({
                message: tracker.message,
                url:     tracker.url
            });
        }

        return result;
    }
};
