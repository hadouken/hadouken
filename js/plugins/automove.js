var session = require("bittorrent").session;
//var config  = require("config");

function getField(fieldName, torrent) {
    if(fieldName === "name") {
        return torrent.name;
    }

    return "";
}

function torrentFinished(torrent) {
    var rules = config.get("extensions.automove.rules");
    
    if(!rules || !rules.length || rules.length === 0) {
        return;
    }
    
    for(var i = 0; i < rules.length; i++) {
        var rule = rules[i];
        var value = getField(rule.field, torrent);
        var regex = new RegExp(rule.pattern);

        if(regex.test(value)) {
            torrent.move(rule.path);
            break;
        }
    }
}

exports.load = function() {
    session.on("torrent.finished", torrentFinished);
}
