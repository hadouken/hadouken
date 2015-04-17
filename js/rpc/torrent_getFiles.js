var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.getFiles",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);
        
        if(!torrent) {
            return null;
        }

        var info = torrent.getTorrentInfo();

        if(!info) {
            return null;
        }

        var files  = info.getFiles();
        var result = [];

        for(var i = 0; i < files.length; i++) {
            var file = files[i];

            result.push({
                index:    i,
                path:     file.path,
                progress: file.progress,
                size:     file.size
            });
        }

        return result;
    }
};
