var bt      = require("bittorrent");
var session = bt.session;

exports.rpc = {
    name: "session.addTorrentUri",
    method: function(uri, params) {
        params = params || {};

        var p    = bt.AddTorrentParams.getDefault();
        var meta = {};
        p.url    = uri;

        if(params.savePath) {
            p.savePath = params.savePath;
        }

        if(params.tags instanceof Array) {
            meta.tags = params.tags;
        }

        if(params.trackers) {
            p.trackers = params.trackers;
        }

        p.metadata = meta;
        return session.addTorrent(p);
    }
};
