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
        return config.obj;
    }
};
