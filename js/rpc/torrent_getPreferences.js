var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.getPreferences",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent) {
            return null;
        }

        return {
            maxConnections:     torrent.maxConnections,
            maxUploads:         torrent.maxUploads,
            resolveCountries:   torrent.resolveCountries,
            sequentialDownload: torrent.sequentialDownload,
            uploadLimit:        torrent.uploadLimit,
            uploadMode:         torrent.uploadMode
        };
    }
};
