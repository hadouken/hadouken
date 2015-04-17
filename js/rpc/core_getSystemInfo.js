var session = require("bittorrent").session;
var core    = require("core");

exports.rpc = {
    name: "core.getSystemInfo",
    method: function() {
        return {
            commitish: core.GIT_COMMIT_HASH,
            branch:    core.GIT_BRANCH,

            versions: {
                libtorrent: session.LIBTORRENT_VERSION,
                hadouken:   core.HADOUKEN_VERSION
            }
        };
    }
};
