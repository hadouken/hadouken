var session = require("bittorrent").session;

function getFeedStatus(url) {
    var feeds  = session.getFeeds();

    for(var i = 0; i < feeds.length; i++) {
        var status = feeds[i].getStatus();

        if(status.url === url) {
            return status;
        }
    }
}

exports.rpc = {
    name: "rss.getFeedItems",
    method: function(url) {
        var result = [];
        var status = getFeedStatus(url);

        if(typeof status === "undefined") {
            throw new Error("Unknown feed url.");
        }

        var items  = status.getItems();

        for(var i = 0; i < items.length; i++) {
            var item = items[i];

            result.push({
                uuid: item.uuid,
                url: item.url,
                title: item.title,
                description: item.description,
                comment: item.comment,
                category: item.category,
                size: item.size
            });
        }

        return result;
    }
};
