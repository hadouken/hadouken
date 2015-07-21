var session = require("bittorrent").session;

exports.rpc = {
    name: "webui.getPeers",
    method: function(infoHash) {
        var torrent = session.findTorrent(infoHash);

        if(!torrent || !torrent.isValid) {
            throw new Error("Invalid info hash: " + infoHash);
        }

        var peers  = torrent.getPeers();
        var result = [];

        for(var i = 0; i < peers.length; i++) {
            var peer = peers[i];
            if((peer.flags & 64) != 0) { continue; }

            var isUtp = ((peer.flags & 131072) != 0);
            var flags = "";
            
            if((peer.flags & 1) != 0 && (peer.flags & 2) == 0) { flags += "D"; } // we are interested, not choked
            if((peer.flags & 1) != 0 && (peer.flags & 2) == 0) { flags += "d"; } // we are interested, choked
            if((peer.flags & 4) != 0 && (peer.flags & 8) == 0) { flags += "U"; } // peer is interested, not choked
            if((peer.flags & 4) != 0 && (peer.flags & 8) != 0) { flags += "u"; } // peer is interested, choked
            if((peer.source & 32) != 0) { flags += "I"; } // incoming connection
            if((peer.flags & 2048) != 0) { flags += "O"; } // optimistic unchoke
            if((peer.flags & 4096) != 0) { flags += "S"; } // snubbed
            if((peer.source & 4) != 0) { flags += "X"; } // obtained via PEX
            if((peer.source & 2) != 0) { flags += "H"; } // obtained via DHT
            if((peer.flags & 1048576) != 0) { flags += "E"; } // encryption (all traffic)
            if((peer.flags & 2097152) != 0) { flags += "e"; } // encryption (handshake)
            if((peer.flags & 131072) != 0) { flags += "P"; } // utp socket
            if((peer.source & 8) != 0) { flags += "L"; } // local peer (lsd)

            result.push([
                peer.country,
                peer.ip,
                "", // rev dns
                isUtp, // utp
                peer.port,
                peer.client,
                flags,
                (peer.progress * 1000) | 0,
                peer.downloadRate,
                peer.uploadRate,
                peer.downloadQueueLength,
                peer.uploadQueueLength,
                -1, // waited
                peer.uploadedBytes,
                peer.downloadedBytes,
                peer.numHashFails, // hash error
                -1, // peer dl
                -1, // max up
                -1, // max down
                -1, // queued
                peer.lastActive, // inactive
                -1  // relevance
            ]);
        }

        return {
            peers: [ infoHash, result ]
        };
    }
};
