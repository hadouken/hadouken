var config  = require("config");
var fs      = require("fs");
var logger  = require("logger").get("plugins.autoadd");
var session = require("bittorrent").session;
var timer   = require("timer");

// Configuration
var defaultPattern = ".*\.torrent$";
var interval       = config.getNumber("extensions.autoadd.interval") || 5;

function getFolders() {
    var key    = "extensions.autoadd.folders";
    var result = [];

    for(var i = 0; i < Number.MAX_VALUE; i++) {
        var query = key + "[" + i + "]";

        if(config.has(query)) {
            var path    = config.getString(query + ".path");
            var pattern = config.getString(query + ".pattern");

            result.push({
                path:    path,
                pattern: new RegExp(pattern || defaultPattern)
            });
        } else {
            break;
        }
    }

    return result;
}

function checkFiles(folder, files) {
    if(!files || files.length === 0) {
        return;
    }

    for(var i = 0; i < files.length; i++) {
        var file = files[i];

        if(folder.pattern.test(file)) {
            logger.info("Adding file " + file);

            session.addTorrentFile(file, {});
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
        var files  = fs.getFiles(folder.path);

        checkFiles(folder, files);
    }
}

function load() {
    var enabled = config.getBoolean("extensions.autoadd.enabled");

    if(!enabled) {
        return;
    }

    var currentTick = 0;
    var folders     = getFolders();

    logger.info("AutoAdd loaded, monitoring " + folders.length + " folders.");

    timer.tick(function() {
        if(currentTick % interval === 0) {
            checkFolders(folders);
        }

        currentTick += 1;
    });
}

exports.load = load;
