var session = require("bittorrent").session;

exports.rpc = {
    name: "misc.getTags",
    method: function() {
        var torrents = session.getTorrents();
        var tags = [];

        for(var i = 0; i < torrents.length; i++) {
            var torrentTags = torrents[i].metadata("tags") || [];

            for(var j = 0; j < torrentTags.length; j++) {
                if(tags.indexOf(torrentTags[j]) < 0) {
                    tags.push(torrentTags[j]);
                }
            }
        }

        return tags;
    }
};
