var bt       = require("bittorrent");
var benc     = require("benc");
var config   = require("config");
var fs       = require("fs");
var logger   = require("logger").get("core");
var rss      = require("rss");
var session  = bt.session;
var timer    = require("timer");

function saveTorrent(torrent) {
    var torrentsPath = getTorrentsPath();
    var file         = fs.combine(torrentsPath, torrent.infoHash + ".torrent");

    if(fs.fileExists(file)) {
        return;
    }

    var info = torrent.getTorrentInfo();

    if(!info) {
        return;
    }

    var creator = new bt.TorrentCreator(info);
    var entry   = creator.generate();
    var buffer  = benc.encode(entry);

    // Save the actual metadata
    fs.writeBuffer(file, buffer);
    logger.info("Saved torrent file for '" + info.name + "' to " + file);
}

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

    saveTorrent(e.torrent);
});

session.on("torrent.add", function(e) {
    if(e.error) {
        logger.error("Could not add torrent (code " + e.error.code + "): " +
                     e.error.message);
        return;
    }

    // Add any metadata to the torrent
    var metadata = e.params.metadata || {};

    if(!metadata["options"]) {
        // Add any default options to the metadata
        var defaultOptions = config.get("bittorrent.defaultOptions") || {};

        var options = {
            seedRatio: (defaultOptions["seedRatio"] || 2.0),
            seedTime: (defaultOptions["seedTime"] || 0),
            seedAction: (defaultOptions["seedAction"] || "pause")
        };

        metadata["options"] = options;
    }

    var keys = Object.keys(metadata);

    for(var i = 0; i < keys.length; i++) {
        e.torrent.metadata(keys[i], metadata[keys[i]]);
    }
});

session.on("torrent.added", function(e) {
    if(!e.torrent.isValid) {
        logger.error("Invalid torrent added.");
        return;
    }

    saveTorrent(e.torrent);
});

session.on("torrent.hashUpdated", function(e) {
    /*
    On migrating metadata
    ---------------------
    When we add a torrent URL (with eg. tags), the info hash
    is not known at the time. We store each metadata object
    in a std::map keyed by the info hash.

    When libtorrent receives the torrent file and gets the correct
    info hash, it sends us this alert.

    We have a special key handler for "_migrate" which takes the
    old info hash as a value, and then moves the metadata from
    the old info hash to the new info hash.

    It is a bit of a hack, but works.
    */

    e.torrent.metadata("_migrate", e.oldInfoHash);
});

session.on("torrent.stateUpdate", function(e) {
    for(var i = 0; i < e.status.length; i++) {
        var status = e.status[i];

        // Only check our seed goal if the torrent is finished
        // and not paused.
        if(!status.isFinished || status.isPaused) {
            return;
        }

        var torrent = status.torrent;
        var options = torrent.metadata("options") || {};

        var goalRatio = options.seedOverride ? (options.seedRatio || 2.0) : 2.0;
        var goalTime  = options.seedOverride ? (options.goalTime  ||   0) :   0;

        if(status.ratio >= parseFloat(goalRatio)
            || (goalTime > 0 && status.seedingTime > goalTime)) {

            logger.info("Torrent " + status.name + " reached its seed goal.");

            var action = (options.seedAction || "pause");

            switch(action) {
                case "pause":
                    torrent.autoManaged = false;
                    torrent.pause();
                    break;

                case "remove":
                    session.removeTorrent(torrent);
                    break;
            }
        }
    }
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
        logger.info("Creating state path: " + statePath);
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

        var params = bt.AddTorrentParams.getDefault();
        params.torrent = new bt.TorrentInfo(file);

        // Load metadata file
        var metadataFile = file.replace(/\.torrent/i, ".metadata");

        if(fs.fileExists(metadataFile)) {
            var data = fs.readText(metadataFile);
            params.metadata = JSON.parse(data);
        }

        // Load resume file
        var resumeFile = file.replace(/\.torrent$/i, ".resume");

        if(fs.fileExists(resumeFile)) {
            params.resumeData = fs.readBuffer(resumeFile);
        }

        session.addTorrent(params);
        logger.info("Torrent '" + params.torrent.name + "' added.");
    }
}

function load() {
    // Set up listen port for BitTorrent communication
    var listenPort = (config.get("bittorrent.listenPort") || 6881);
    if(isNaN(listenPort)) {
        logger.warn("Invalid listen port specified. Using default (6881).");
        listenPort = 6881;
    }

    session.listenOn([listenPort, listenPort]);

    // Load country database
    var geoIp = config.getString("bittorrent.geoIpFile");

    if(geoIp) {
        logger.info("Loading GeoIP database " + geoIp);
        session.loadCountryDb(geoIp);
    }

    // Add DHT routers.
    var routers = config.get("bittorrent.dht.routers") || [];
    for(var i = 0; i < routers.length; i++) {
        session.addDhtRouter(routers[i][0], routers[i][1]);
    }

    // Enable DHT
    if(config.get("bittorrent.dht.enabled")) {
        logger.info("Enabling DHT.");
        session.startDht();
    }

    // Enable LSD
    if(config.get("bittorrent.lsd.enabled")) {
        logger.info("Enabling LSD.");
        session.startLsd();
    }

    // Enable NatPmp
    if(config.get("bittorrent.natpmp.enabled")) {
        logger.info("Enabling NATPMP.");
        session.startNatPmp();
    }

    // Enable UPnP
    if(config.get("bittorrent.upnp.enabled")) {
        logger.info("Enabling UPnP.");
        session.startUpnp();
    }

    // Load session settings
    var settings = session.getSettings();
    settings.downloadRateLimit = (config.get("bittorrent.downloadRateLimit") || settings.downloadRateLimit);
    settings.mixedModeAlgorithm = (config.get("bittorrent.mixedModeAlgorithm") || settings.mixedModeAlgorithm);
    settings.rateLimitIpOverhead = (config.get("bittorrent.rateLimitIpOverhead") || settings.rateLimitIpOverhead);
    settings.rateLimitUtp = (config.get("bittorrent.rateLimitUtp") || settings.rateLimitUtp);
    settings.uploadRateLimit = (config.get("bittorrent.uploadRateLimit") || settings.uploadRateLimit);

    // Advanced settings
    var allowMultipleConnectionsPerIp = config.get("bittorrent.allowMultipleConnectionsPerIp");
    if(typeof allowMultipleConnectionsPerIp !== "undefined") { settings.allowMultipleConnectionsPerIp = allowMultipleConnectionsPerIp; }

    var anonymousMode = config.get("bittorrent.anonymousMode");
    if(typeof anonymousMode !== "undefined") { settings.anonymousMode = anonymousMode; }

    settings.halfOpenLimit = (config.get("bittorrent.halfOpenLimit") || settings.halfOpenLimit);

    session.setSettings(settings);

    logger.info("Loaded session settings.");

    loadState();
    loadTorrents();

    rss.load();
    require("plugins").load();

    // Post torrent updates each tick of the timer.
    timer.tick(function() { session.postTorrentUpdates(); });
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
            var data = alert.torrent.metadata();

            if(data) {
                var metadataFile = fs.combine(torrentsPath, alert.torrent.infoHash + ".metadata");
                fs.writeText(metadataFile, JSON.stringify(data));
            }

            fs.writeBuffer(resumeFile, buffer);
            logger.info("Saved resume data for '" + alert.torrent.getTorrentInfo().name + "'");
        }
    }
}

function saveConfig() {
    // This will move the hadouken.json file we loaded *this*
    // session to hadouken.json.old, and save our current config
    // as hadouken.json. Safeguarding against failures.

    if(fs.fileExists(__CONFIG__)) {
        var old = fs.combine(__CONFIG_PATH__, "hadouken.json.old");
        fs.rename(__CONFIG__, old);
        logger.info("Configuration backed up.");
    }

    var data = JSON.stringify(config.get(), null, "  ");
    fs.writeText(__CONFIG__, data);
}

function unload() {
    rss.unload();

    saveState();
    saveTorrents();
    saveConfig();
}

exports.load = load;
exports.unload = unload;
