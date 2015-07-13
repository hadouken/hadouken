var bt      = require("bittorrent");
var session = bt.session;

function perform(action, torrent) {
    switch(action) {
        case "forcestart":
            torrent.autoManaged = false;
            torrent.resume();
            break;

        case "pause":
            torrent.pause();
            break;

        case "queuebottom":
            torrent.queueBottom();
            break;

        case "queuedown":
            torrent.queueDown();
            break;

        case "queuetop":
            torrent.queueTop();
            break;

        case "queueup":
            torrent.queueUp();
            break;

        case "recheck":
            torrent.forceRecheck();
            break;

        case "remove":
            session.removeTorrent(torrent, false);
            break;

        case "removedata":
            session.removeTorrent(torrent, true);
            break;

        case "start":
            torrent.resume();
            break;

        default:
            throw new Error("Unknown action: " + action);
    }
}

exports.rpc = {
    name: "webui.perform",
    method: function(action, hashList) {
        for(var i = 0; i < hashList.length; i++) {
            var torrent = session.findTorrent(hashList[i]);
            
            if(!torrent || !torrent.isValid) {
                throw new Error("Invalid info hash: " + infoHash);
            }

            perform(action, torrent);
        }
    }
};
