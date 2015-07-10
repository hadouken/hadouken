var bt      = require("bittorrent");
var session = bt.session;

exports.rpc = {
    name: "session.addTorrentFile",
    method: function(data, params) {
        params = params || {};

        var buffer = Duktape.dec("base64", data);
        var p      = bt.AddTorrentParams.getDefault();
        var meta   = {};

        p.torrent  = new bt.TorrentInfo(buffer);

        if(params.filePriorities instanceof Array) {
            p.filePriorities = params.filePriorities;
        }

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
