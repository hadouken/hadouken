var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.getMetadata",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent) {
            return null;
        }

        var data = {};
        var keys = torrent.metadata.keys;

        for(var i = 0; i < keys.length; i++) {
            data[keys[i]] = torrent.metadata.get(keys[i]);
        }

        return data;
    }
};
