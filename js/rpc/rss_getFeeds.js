var session = require("bittorrent").session;

exports.rpc = {
    name: "rss.getFeeds",
    method: function() {
        var feeds  = session.getFeeds();
        var result = {};

        for(var i = 0; i < feeds.length; i++) {
            var status = feeds[i].getStatus();

            result[status.url] = {
                url: status.url,
                title: status.title,
                description: status.description,
                lastUpdate: status.lastUpdate,
                nextUpdate: status.nextUpdate,
                isUpdating: status.isUpdating,
                items: status.getItems().length
            };
        }

        return result;
    }
};
