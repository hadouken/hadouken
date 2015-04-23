var session = require("bittorrent").session;

exports.rpc = {
    name: "session.pause",
    method: function() {
        session.pause();
        return true;
    }
};
