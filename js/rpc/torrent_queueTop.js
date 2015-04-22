var session = require("bittorrent").session;
var core    = require("core");

exports.rpc = {
    name: "torrent.queueTop",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent) {
            return false;
        }

        torrent.queueTop();
        return true;
    }
};
