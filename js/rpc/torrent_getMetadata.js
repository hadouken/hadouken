var session  = require("bittorrent").session;
var metadata = require("metadata");

exports.rpc = {
    name: "torrent.getMetadata",
    method: function(infoHash, key) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        if(key) {
            var val = metadata.get(infoHash, key);

            if(typeof val === 'undefined') {
                throw new Error("Key not found: " + key);
            }

            return val;
        }

        return metadata.getAll(infoHash);
    }
};
