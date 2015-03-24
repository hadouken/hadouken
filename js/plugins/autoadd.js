var session = require("bittorrent").session;
var config  = require("config");
var fs      = require("fs");

// Local variables
var timer           = null;
var defaultInterval = 5000;

function checkFolders() {
    var folders = config.get("autoadd.folders");

    if(!folders || !folders.length || folders.length === 0) {
        return;
    }

    for(var i = 0; i < folders.length; i++) {
        var folder = folders[i];
        var files  = fs.getFiles(folder.path);

        if(!files || !files.length || files.length === 0) {
            continue;
        }

        for(var j = 0; j < files.length; j++) {
            var file = files[j];

            if(!file.endsWith(".torrent")) {
                continue;
            }

            var params = {};
            if(folder.savePath) params.savePath = folder.savePath;

            session.addTorrentFile(file, params);
            fs.removeFile(file);
        }
    }
}

exports.load = function() {
    var interval = config.get("autoadd.interval") || defaultInterval;
    timer = setInterval(checkFolders, interval);
}

exports.unload = function() {
    clearInterval(timer);
}
