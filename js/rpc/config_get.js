var config = require("config");

function get(key, fn) {
    if(config.has(key)) {
        return fn.bind(config)(key);
    }
}

function getDhtRouters() {
    var routers = "bittorrent.dht.routers";
    var result = [];

    for(var i = 0; i < Number.MAX_VALUE; i++) {
        var query = routers + "[" + i + "]";

        if(config.has(query)) {
            var host = config.getString(query + "[0]");
            var port = config.getNumber(query + "[1]");

            result.push([ host, port ]);
        } else {
            break;
        }
    }

    return result;
}

exports.rpc = {
    name: "config.get",
    method: function() {
        return {
            bittorrent: {
                anonymousMode: get("bittorrent.anonymousMode", config.getBoolean),
                defaultSavePath: get("bittorrent.defaultSavePath", config.getString),
                dht: {
                    enabled: get("bittorrent.dht.enabled", config.getBoolean),
                    routers: getDhtRouters()
                },
                statePath: get("bittorrent.statePath", config.getString)
            },
            http: {
                port: get("http.port", config.getNumber),
                root: get("http.root", config.getString)
            }
        };
    }
};
