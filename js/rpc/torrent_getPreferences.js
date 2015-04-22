var session = require("bittorrent").session;
var core    = require("core");

exports.rpc = {
    name: "torrent.getPreferences",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent) {
            return null;
        }

        return {
            maxConnections:   torrent.maxConnections,
            maxUploads:       torrent.maxUploads,
            resolveCountries: torrent.resolveCountries,
            uploadLimit:      torrent.uploadLimit,
            uploadMode:       torrent.uploadMode
        };
    }
};
