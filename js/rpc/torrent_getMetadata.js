var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.getMetadata",
    method: function(infoHash, key) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            return null;
        }

        if(key) {
            var val = torrent.metadata(key);

            if(typeof val === 'undefined') {
                throw new Error("Key not found: " + key);
            }
        }

        return torrent.metadata();
    }
};
