var bt       = require("bittorrent");
var benc     = require("benc");
var config   = require("config");
var fs       = require("fs");
var logger   = require("logger").get("core");
var metadata = require("metadata");
var session  = bt.session;

session.on("torrent.removed", function(e) {
    var torrentsPath = getTorrentsPath();

    var torrentFile  = fs.combine(torrentsPath, e.infoHash + ".torrent");
    var resumeFile   = fs.combine(torrentsPath, e.infoHash + ".resume");
    var metadataFile = fs.combine(torrentsPath, e.infoHash + ".metadata");

    if(fs.fileExists(torrentFile)) {
        fs.deleteFile(torrentFile);
    }

    if(fs.fileExists(resumeFile)) {
        fs.deleteFile(resumeFile);
    }

    if(fs.fileExists(metadataFile)) {
        fs.deleteFile(metadataFile);
    }
});

session.on("torrent.metadataReceived", function(e) {
    if(!e.torrent.isValid) {
        logger.error("Received metadata for an invalid torrent.");
        return;
    }

    var info    = e.torrent.getTorrentInfo();
    var creator = new bt.TorrentCreator(info);
    var entry   = creator.generate();
    var buffer  = benc.encode(entry);

    // Save the actual metadata
    var torrentsPath = getTorrentsPath();
    var file         = fs.combine(torrentsPath, e.torrent.infoHash + ".torrent");

    fs.writeBuffer(file, buffer);
    logger.info("Saved torrent file for '" + e.torrent.getTorrentInfo().name + "' to " + file);
});

function getTorrentsPath() {
    var statePath = config.getString("bittorrent.statePath") || "state";
    var torrentsPath = fs.combine(statePath, "torrents");

    if(!fs.directoryExists(torrentsPath)) {
        fs.createDirectories(torrentsPath);
    }

    return torrentsPath;
}

function loadState() {
    var statePath = config.getString("bittorrent.statePath") || "state";

    if(!fs.directoryExists(statePath)) {
        logger.info("creating state path");
        fs.createDirectories(statePath);
    }

    var stateFile = fs.combine(statePath, ".session_state");

    if(!fs.fileExists(stateFile)) {
        logger.info("No previous session state to load.");
        return;
    }

    var data = fs.readBuffer(stateFile);
    var decoded = benc.decode(data);

    // load session state
    session.loadState(decoded);
    logger.info("Loaded session state from '" + stateFile + "'");
}

function loadTorrents() {
    var torrentsPath = getTorrentsPath();
    var files = fs.getFiles(torrentsPath);

    for(var i = 0; i < files.length; i++) {
        var file = files[i];
        if(!file.endsWith(".torrent")) { continue; }

        var params = new bt.AddTorrentParams.getDefault();
        params.torrent = new bt.TorrentInfo(file);

        // Load resume file
        var resumeFile = file.replace(/\.torrent$/i, ".resume");
        
        if(fs.fileExists(resumeFile)) {
            params.resumeData = fs.readBuffer(resumeFile);
        }

        // Load metadata file
        var metadataFile = file.replace(/\.torrent$/i, ".metadata");

        if(fs.fileExists(metadataFile)) {
            var data = JSON.parse(fs.readText(metadataFile));
            print(data);
            print(params.torrent.infoHash);
            metadata.replace(params.torrent.infoHash, data);
        }

        session.addTorrent(params);
        logger.info("Torrent '" + params.torrent.name + "' added.");
    }
}

function load() {
    // Set up listen port for BitTorrent communication
    session.listenOn([6881, 6889]);

    loadState();
    loadTorrents();

    require("plugins").load();
}

function saveState() {
    var statePath = config.getString("bittorrent.statePath") || "state";
    var stateFile = fs.combine(statePath, ".session_state");

    var entry  = session.saveState();
    var buffer = benc.encode(entry);

    fs.writeBuffer(stateFile, buffer);
    logger.info("Saved session state to '" + stateFile + "'");
}

function saveTorrents() {
    session.pause();

    var torrentsPath = getTorrentsPath();
    var num          = 0;
    var torrents     = session.getTorrents();

    for(var i = 0; i < torrents.length; i++) {
        var t = torrents[i];
        if(!t.isValid) { continue; }

        var s = t.getStatus();

        if(!s.hasMetadata) { continue; }
        if(!s.needSaveResume) { continue; }

        num += 1;
        t.saveResumeData();
    }

    logger.info("Saving resume data for " + num + " torrent(s)");

    while(num > 0) {
        var a = session.waitForAlert(10 * 1000);
        if(!a) { continue; }

        var alerts = session.getAlerts();

        for(var i = 0; i < alerts.length; i++) {
            var alert = alerts[i];

            if(alert == null) {
                continue;
            }

            if(alert.name === "torrent_paused_alert") {
                continue;
            }

            if(alert.name === "save_resume_data_failed_alert") {
                num -= 1;
                continue;
            }

            if(alert.name !== "save_resume_data_alert") {
                continue;
            }

            num -= 1;

            if(!alert.resumeData) {
                logger.info("No resume data for torrent.");
                continue;
            }

            var buffer = benc.encode(alert.resumeData);
            var resumeFile = fs.combine(torrentsPath, alert.torrent.infoHash + ".resume");

            var data = metadata.getAll(alert.torrent.infoHash);
            if(data) {
                var metadataFile = fs.combine(torrentsPath, alert.torrent.infoHash + ".metadata");
                fs.writeText(metadataFile, JSON.stringify(data));
            }

            fs.writeBuffer(resumeFile, buffer);
            logger.info("Saved resume data for '" + alert.torrent.getTorrentInfo().name + "'");
        }
    }
}

function unload() {
    saveState();
    saveTorrents();
}

exports.load = load;
exports.unload = unload;
