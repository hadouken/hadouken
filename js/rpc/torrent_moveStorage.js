var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.moveStorage",
    method: function(infoHash, path) {
        var torrent = session.findTorrent(infoHash);

        if(torrent) {
            torrent.moveStorage(path);
            return true;
        }

        return false;
    }
};
