var session = require("bittorrent").session;

function explodeTrackers(trackers) {
    var result = "";
    var tiers = [];

    for(var i = 0; i < trackers.length; i++) {
        var tier = trackers[i].tier;

        if(tier >= tiers.length) {
            tiers = tiers.slice(0, tier + 1);
            tiers[tier] = [];
        }

        tiers[tier].push(trackers[i].url);
    }

    for(var i = 0; i < tiers.length; i++) {
        for(var j = 0; j < tiers[i].length; j++) {
            result += tiers[i][j] + "\r\n";
        }
    }

    return result;
}

function getProperties(infoHash) {
    var torrent = session.findTorrent(infoHash);

    if(!torrent || !torrent.isValid) {
        throw new Error("Invalid info hash: " + infoHash);
    }

    var status = torrent.getStatus();
    var trackers = torrent.getTrackers();
    var trackerResult = explodeTrackers(trackers);

    var options = torrent.metadata("options") || {};

    return {
        hash: torrent.infoHash,
        auto_managed: torrent.autoManaged,
        save_path: status.savePath,
        trackers: trackerResult,
        ulrate: torrent.uploadLimit,
        dlrate: torrent.downloadLimit,
        superseed: torrent.superSeed ? 1 : 0,
        dht: 1,
        pex: 1,
        seed_override: !!(options.seedOverride),
        seed_ratio: (options.seedRatio || 2.0) * 1000,
        seed_time: ((options.seedTime || 0) / 60) | 0,
        ulslots: (torrent.maxUploads >= 16777215 ? -1 : torrent.maxUploads),
        seed_num: 0,
        autoexec: 0,
        autoexec_hide_ui: 0,
        perf: {
            w: 0,
            W: 0,
            h: 0,
            c: 2,
            j: 1223123
        }
    };
}

exports.rpc = {
    name: "webui.getProperties",
    method: function(infoHash) {
        // could probably be a list of info hashes
        var result = getProperties(infoHash);

        return {
            props: [ result ]
        };
    }
};
