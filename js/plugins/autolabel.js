var bt      = require("bittorrent");
var config  = require("config");
var logger  = require("logger").get("plugins.autolabel");
var session = bt.session;

function isMatch(torrent, field, pattern) {
    switch(field) {
        case "name":
            var status = torrent.getStatus();
            return pattern.test(status.name);

        case "tracker":
            var trackers = torrent.getTrackers();
            for(var i = 0; i < trackers.length; i++) {
                if(pattern.test(trackers[i].url)) {
                    return true;
                }
            }
            break;
        default:
            return false;
    }

    return false;
}

function torrentAdded(rules, torrent) {
    if(!torrent.isValid) {
        return;
    }

    var existingLabel = (torrent.metadata("label") || "");

    if(existingLabel || existingLabel !== "") {
        // Do not label torrents with an existing label.
        return;
    }

    for(var i = 0; i < rules.length; i++) {
        var rule = rules[i];

        if(!rule.field || !rule.pattern || !rule.label) {
            logger.warn("Found invalid AutoLabel rule.");
            continue;
        }

        if(isMatch(torrent, rule.field, new RegExp(rule.pattern))) {
            logger.info("Updated label on '" +
                        torrent.getStatus().name +
                        "' to '" +
                        rule.label +
                        "'.");

            torrent.metadata("label", rule.label);
            break;
        }
    }
}

function load() {
    var enabled = config.getBoolean("extensions.autolabel.enabled");
    
    if(!enabled) {
        return;
    }

    var currentTick = 0;
    var rules       = config.get("extensions.autolabel.rules");

    session.on("torrent.add", function(args) {
        torrentAdded(rules, args.torrent);
    });

    logger.info("AutoLabel loaded with " + rules.length + " rules.");
}

exports.load = load;
