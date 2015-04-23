var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.renameFile",
    method: function(infoHash, fileIndex, fileName) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent) {
            return null;
        }

        torrent.renameFile(parseInt(fileIndex, 10), fileName);
        return true;
    }
};
