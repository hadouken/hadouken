var session = require("bittorrent").session;
var timer   = null;

function printTorrents() {
    var torrents = session.getTorrents();
    print(torrents.length + " torrents:");

    for(var i = 0; i < torrents.length; i++) {
        var torrent = torrents[i];
        var status = torrent.getStatus();

        print("  Name: " + status.name);
        print("  Progress: " + status.progress * 100 + "%");
    }
}

exports.load = function() {
    timer = setInterval(printTorrents, 1000);
}
