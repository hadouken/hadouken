var config  = require("config");
var http    = require("http");
var logger  = require("logger").get("plugins.pushover");
var session = require("bittorrent").session;

// Configuration
var url   = "https://api.pushover.net/1/messages.json";
var token = config.getString("extensions.pushover.token");
var user  = config.getString("extensions.pushover.user");

function pushMessage(title, message) {
    var data  = "token=" + token;
        data += "&user=" + user;
        data += "&title=" + encodeURI(title);
        data += "&message=" + encodeURI(message);

    var response = http.post(url, data, {
        headers: {
            "Content-Type":  "application/x-www-form-urlencoded"
        }
    });

    if(response.status !== 200) {
        logger.error("Failed to push message to Pushover: " + response.reason);
    }
}

function load() {
    var enabled = config.getBoolean("extensions.pushover.enabled");

    if(!enabled) {
        return;
    }

    if(!token || !user) {
        logger.warn("No token/user configured.");
        return;
    }

    var events = config.get("extensions.pushover.enabledEvents");
    logger.info("Pushover enabled with " + events.length + " enabled events.");

    if(events.indexOf("torrent.added") > -1) {
        session.on("torrent.added", function(args) {
            var status = args.torrent.getStatus();
            pushMessage("Torrent added", "Torrent '" + status.name + "' was added.");
        });
    }

    if(events.indexOf("torrent.finished") > -1) {
        session.on("torrent.finished", function(args) {
            var status = args.torrent.getStatus();
            pushMessage("Torrent finished", "Torrent '" + status.name + "' finished downloading.");
        });
    }

    if(events.indexOf("hadouken.loaded") > -1) {
        pushMessage("Hadouken loaded", "Hadouken just started up.");
    }
}

exports.load = load;
