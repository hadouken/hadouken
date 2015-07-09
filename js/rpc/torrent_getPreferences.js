var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.getPreferences",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
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
