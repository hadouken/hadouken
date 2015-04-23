var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.forceRecheck",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent) {
            return null;
        }

        torrent.forceRecheck();
        return true;
    }
};
