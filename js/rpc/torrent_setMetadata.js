var session  = require("bittorrent").session;

exports.rpc = {
    name: "torrent.setMetadata",
    method: function(infoHash, key, value) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        torrent.metadata(key, value);
        return true;
    }
};
