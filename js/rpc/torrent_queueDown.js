var session = require("bittorrent").session;
var core    = require("core");

exports.rpc = {
    name: "torrent.queueDown",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent) {
            return false;
        }

        torrent.queueDown();
        return true;
    }
};
