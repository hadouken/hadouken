var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.renameFile",
    method: function(infoHash, fileIndex, fileName) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        torrent.renameFile(parseInt(fileIndex, 10), fileName);
        return true;
    }
};
