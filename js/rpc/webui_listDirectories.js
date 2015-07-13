var config  = require("config");
var fs      = require("fs");
var session = require("bittorrent").session;

exports.rpc = {
    name: "webui.listDirectories",
    method: function() {
        var dirs = (config.get("bittorrent.downloadDirectories") || []).slice();
        var result = [];

        // Add the default save path to the result. It is translated to
        // "Default download directory" in the UI.
        dirs.unshift(config.getString("bittorrent.defaultSavePath") || ".");

        for(var i = 0; i < dirs.length; i++) {
            if(!fs.directoryExists(dirs[i])) {
                continue;
            }

            var space = fs.space(dirs[i]);

            result.push({
                available: space.available,
                path: dirs[i]
            });
        }

        return {
            "download-dirs": result
        };
    }
};
