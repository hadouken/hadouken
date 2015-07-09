var session = require("bittorrent").session;

exports.rpc = {
    name: "torrent.getPeers",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);
        
        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        var peers  = torrent.getPeers();
        var result = [];

        for(var i = 0; i < peers.length; i++) {
            var peer = peers[i];

            result.push({
                country:         peer.country,
                ip:              peer.ip,
                port:            peer.port,
                connectionType:  peer.connectionType,
                client:          peer.client,
                progress:        peer.progress,
                downloadRate:    peer.downloadRate,
                uploadRate:      peer.uploadRate,
                downloadedBytes: peer.downloadedBytes,
                uploadedBytes:   peer.uploadedBytes
            });
        }

        return result;
    }
};
