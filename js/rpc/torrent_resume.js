var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.resume",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(torrent) {
            torrent.resume();
            return true;
        }

        return false;
    }
};
