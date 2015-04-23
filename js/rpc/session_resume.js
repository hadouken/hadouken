var session = require("bittorrent").session;

exports.rpc = {
    name: "session.resume",
    method: function() {
        session.resume();
        return true;
    }
};
