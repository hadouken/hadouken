var bt      = require("bittorrent");
var session = bt.session;

exports.rpc = {
    name: "session.addTorrentUri",
    method: function(uri, params) {
        var p  = bt.AddTorrentParams.getDefault();
        p.url  = uri;

        if(params.savePath) {
            p.savePath = params.savePath;
        }

        return session.addTorrent(p);
    }
};
