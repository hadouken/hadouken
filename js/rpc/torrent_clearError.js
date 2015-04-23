var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.clearError",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent) {
            return null;
        }

        torrent.clearError();
        return true;
    }
};
