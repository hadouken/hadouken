var session = require("bittorrent").session;
var config  = require("config");
var fs      = require("fs");

var timer   = null;

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

            session.addTorrentFile(file, { savePath: folder.savePath });
            fs.removeFile(file);
        }
    }
}

exports.load = function() {
    timer = setInterval(checkFolders, 5000);
}

exports.unload = function() {
    clearInterval(timer);
}
