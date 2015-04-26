var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.setMetadata",
    method: function(infoHash, key, value) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent) {
            return false;
        }

        torrent.metadata.set(key, value);
        return true;
    }
};
