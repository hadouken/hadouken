var session = require("bittorrent").session;

function setProperty(torrent, key, val) {
    switch(key) {
        case "auto_managed":
            torrent.autoManaged = !!(val);
            break;

        case "dlrate":
            val = parseInt(val, 10);
            if(isNaN(val)) { return; }
            torrent.downloadLimit = val;
            break;

        case "label":
            if(typeof val === "string") {
                torrent.metadata("label", val);
            }
            break;

        case "save_path":
            if(typeof val === "string") {
                torrent.moveStorage(val);
            }
            break;

        case "seed_override":
            var opts = torrent.metadata("options");
            opts.seedOverride = !!(val);
            torrent.metadata("options", opts);
            break;

        case "seed_ratio":
            val = parseInt(val, 10);
            if(isNaN(val)) { return; }

            var opts = torrent.metadata("options");
            opts.seedRatio = val / 1000;
            torrent.metadata("options", opts);
            break;

        case "seed_time":
            val = parseInt(val, 10);
            if(isNaN(val)) { return; }

            var opts = torrent.metadata("options");
            opts.seedTime = (val * 60);
            torrent.metadata("options", opts);
            break;

        case "superseed":
            torrent.superSeed = !!(val);
            break;

        case "trackers":
            break;

        case "ulrate":
            val = parseInt(val, 10);
            if(isNaN(val)) { return; }
            torrent.uploadLimit = val;
            break;

        case "ulslots":
            val = parseInt(val, 10);
            if(isNaN(val)) { return; }
            torrent.maxUploads = val;
            break;
    }
}

function setProperties(torrent, props) {
    var keys = Object.keys(props);

    for(var i = 0; i < keys.length; i++) {
        setProperty(torrent, keys[i], props[keys[i]]);
    }
}

exports.rpc = {
    name: "webui.setProperties",
    method: function(hashList, props) {
        if(typeof hashList === "string") {
            hashList = [ hashList ];
        }

        for(var i = 0; i < hashList.length; i++) {
            var torrent = session.findTorrent(hashList[i]);

            if(!torrent || !torrent.isValid) {
                throw new Error("Invalid info hash: " + hashList[i]);
            }

            setProperties(torrent, props || {});
        }
    }
};
