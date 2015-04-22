var session = require("bittorrent").session;
var core    = require("core");

exports.rpc = {
    name: "torrent.queueBottom",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent) {
            return false;
        }

        torrent.queueBottom();
        return true;
    }
};
