var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.queueDown",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        torrent.queueDown();
        return true;
    }
};
