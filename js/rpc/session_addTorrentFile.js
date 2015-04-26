var session = require("bittorrent").session;

exports.rpc = {
    name: "session.addTorrentFile",
    method: function(data, params) {
        var buffer = Duktape.dec("base64", data);
        return session.addTorrent(buffer, params || {});
    }
};
