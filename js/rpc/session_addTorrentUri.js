var session = require("bittorrent").session;

exports.rpc = {
    name: "session.addTorrentUri",
    method: function(uri, params) {
        params = params || {};
        return session.addTorrentUri(uri, params);
    }
};
