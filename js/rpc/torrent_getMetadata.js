var session  = require("bittorrent").session;

exports.rpc = {
    name: "torrent.getMetadata",
    method: function(infoHash, key) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        if(key) {
            return torrent.metadata(key);
        }

        return torrent.metadata();
    }
};
