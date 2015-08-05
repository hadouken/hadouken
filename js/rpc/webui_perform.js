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
            var status = torrent.getStatus();

            if(!status.hasMetadata) {
                return;
            }

            // If the torrent is paused and *not* automanaged,
            // set a flag to pause if after recheck and then
            // resume it so the recheck can start.

            if(status.isPaused && !torrent.autoManaged) {
                torrent.metadata("_pauseAfterRecheck", true);
                torrent.resume();
            }

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
