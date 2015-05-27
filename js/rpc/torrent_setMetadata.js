var session  = require("bittorrent").session;
var metadata = require("metadata");

exports.rpc = {
    name: "torrent.setMetadata",
    method: function(infoHash, key, value) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        metadata.set(infoHash, key, value);
        return true;
    }
};
