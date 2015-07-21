var logger  = require("logger").get("webui.setFilePriority");
var session = require("bittorrent").session;

exports.rpc = {
    name: "webui.setFilePriority",
    method: function(infoHash, files, priority) {
        if(!files || !files.length) {
            return;
        }

        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        var status = torrent.getStatus();

        if(status.isSeeding || status.isFinished) {
            logger.warn("Torrent '" + status.name + "' is seeding/finished. " +
                        "Cannot set file priority.");
            return;
        }

        for(var i = 0; i < files.length; i++) {
            torrent.setFilePriority(files[i], priority);
        }
    }
};
