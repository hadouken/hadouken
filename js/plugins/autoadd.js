var bt      = require("bittorrent");
var config  = require("config");
var fs      = require("fs");
var logger  = require("logger").get("plugins.autoadd");
var session = bt.session;
var timer   = require("timer");

// Configuration
var defaultPattern = ".*\.torrent$";
var interval       = config.getNumber("extensions.autoadd.interval") || 5;

function checkFiles(folder, files) {
    if(!files || files.length === 0) {
        return;
    }

    var pattern = new RegExp(folder.pattern || defaultPattern);

    for(var i = 0; i < files.length; i++) {
        var file = files[i];

        if(pattern.test(file)) {
            var buffer = fs.readBuffer(file);
            var p      = bt.AddTorrentParams.getDefault();
            p.torrent  = new bt.TorrentInfo(buffer);

            if(folder.savePath) {
                p.savePath = folder.savePath;
            }

            logger.info("(AUTOADD) Adding torrent " + p.torrent.name + " from file " + file);

            session.addTorrent(p);
            fs.deleteFile(file);
        }
    }
}

function checkFolders(folders) {
    if(!folders || folders.length === 0) {
        return;
    }

    for(var i = 0; i < folders.length; i++) {
        var folder = folders[i];
        
        if(!fs.directoryExists(folder.path)) {
            return;
        }

        var files = fs.getFiles(folder.path);
        checkFiles(folder, files);
    }
}

function load() {
    var enabled = config.getBoolean("extensions.autoadd.enabled");
    
    if(!enabled) {
        return;
    }

    var currentTick = 0;
    var folders     = config.get("extensions.autoadd.folders");

    logger.info("AutoAdd loaded, monitoring " + folders.length + " folders.");

    timer.tick(function() {
        if(currentTick % interval === 0) {
            checkFolders(folders);
        }

        currentTick += 1;
    });
}

exports.load = load;
