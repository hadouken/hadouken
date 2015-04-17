var session = require("bittorrent").session;

exports.rpc = {
    name: "session.removeTorrent",
    method: function(infoHash, removeData) {
        if(session.findTorrent(infoHash)) {
            return session.removeTorrent(infoHash, removeData || false);
        }

        return false;
    }
};
