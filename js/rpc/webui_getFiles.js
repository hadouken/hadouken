var session = require("bittorrent").session;

exports.rpc = {
    name: "webui.getFiles",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        var info = torrent.getTorrentInfo();

        if(!info) {
            return {files:[]};
        }

        var files      = info.getFiles();
        var progress   = torrent.getFileProgress();
        var priorities = torrent.getFilePriorities();
        var result = [];

        for(var i = 0; i < files.length; i++) {
            var file = files[i];

            result.push([
                file.path,
                file.size,
                progress[i],
                priorities[i],
                -1, // first piece
                -1, // num pieces
                -1, // streamable
                -1, // encoded rate
                -1, // duration
                -1, // width,
                -1, // height
                -1, // stream eta
                -1 // streamability
            ]);
        }

        return {
            files: [ infoHash, result ]
        };
    }
};
