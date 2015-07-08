var session = require("bittorrent").session;

exports.rpc = {
    name: "session.removeTorrent",
    method: function(infoHash, removeData) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        session.removeTorrent(torrent, removeData || false);
        return true;
    }
};
