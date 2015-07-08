var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.setPreferences",
    method: function(infoHash, prefs) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        prefs = prefs || {};

        if(prefs.hasOwnProperty("maxConnections")) { torrent.maxConnections = parseInt(prefs.maxConnections, 10); }
        if(prefs.hasOwnProperty("maxUploads")) { torrent.maxUploads = parseInt(prefs.maxUploads, 10); }
        if(prefs.hasOwnProperty("resolveCountries")) { torrent.resolveCountries = !!prefs.resolveCountries; }
        if(prefs.hasOwnProperty("sequentialDownload")) { torrent.sequentialDownload = !!prefs.sequentialDownload; }
        if(prefs.hasOwnProperty("uploadLimit")) { torrent.uploadLimit = parseInt(prefs.uploadLimit, 10); }
        if(prefs.hasOwnProperty("uploadMode")) { torrent.uploadMode = !!prefs.uploadMode; }

        return true;
    }
};
