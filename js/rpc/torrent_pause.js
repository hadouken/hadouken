var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.pause",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(torrent) {
            torrent.pause();
            return true;
        }

        return false;
    }
};
