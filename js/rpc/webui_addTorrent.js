var bt      = require("bittorrent");
var config  = require("config");
var fs      = require("fs");
var session = bt.session;

exports.rpc = {
    name: "webui.addTorrent",
    method: function(type, data, params) {
        type   = type   || "file";
        params = params || {};
        
        var p      = bt.AddTorrentParams.getDefault();
        var meta   = {};

        if(type === "file") {
            var buffer = Duktape.dec("base64", data);
            p.torrent  = new bt.TorrentInfo(buffer);
        } else if(type === "url") {
            p.url = data;
        }

        if(params.filePriorities instanceof Array) {
            p.filePriorities = params.filePriorities;
        }

        if(typeof params.label === "string") {
            meta.label = params.label;
        }

        if(!!(params.paused)) {
            p.flags &= ~64; // unset flag_auto_managed
            p.flags |= 32; // flag_paused
        }

        if(!!(params.sequentialDownload)) {
            p.flags |= 2048;
        }

        // Save path is an index referring to
        // [0] the default save path
        // [n+1] paths in the "bittorrent.downloadDirectories" array

        if(params.savePath) {
            var idx = parseInt(params.savePath);

            if(!isNaN(idx) && idx > 0) {
                var dirs = config.get("bittorrent.downloadDirectories") || [];

                if(idx < dirs.length) {
                    p.savePath = dirs[idx - 1];
                }
            }
        }

        if(params.subPath) {
            p.savePath = fs.combine(p.savePath, params.subPath);
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
