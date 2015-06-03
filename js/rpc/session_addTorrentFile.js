var bt      = require("bittorrent");
var session = bt.session;

exports.rpc = {
    name: "session.addTorrentFile",
    method: function(data, params) {
        var buffer = Duktape.dec("base64", data);
        var p      = bt.AddTorrentParams.getDefault();
        p.torrent  = new bt.TorrentInfo(buffer);

        if(params.savePath) {
            p.savePath = params.savePath;
        }

        return session.addTorrent(p);
    }
};
