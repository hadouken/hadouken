var config  = require("config");
var logger  = require("logger").get("plugins.automove");
var session = require("bittorrent").session;

function Rule(path, filter) {
    this.path   = path;
    this.filter = filter;
}

function PatternFilter(regex, field) {
    this._regex = regex;
    this._field = field;
}

PatternFilter.prototype.isMatch = function(torrent) {
    var status = torrent.getStatus();

    if(this._field === "name") {
        return this._regex.test(status.name);
    }

    return false;
}

function TagsFilter(tags) {
    this._tags = tags;
}

TagsFilter.prototype.isMatch = function(torrent) {
    var torrentTags = torrent.metadata("tags");
    if(!torrentTags) { return false; }

    for(var i = 0; i < this._tags.length; i++) {
        var tag = this._tags[i];

        if(torrentTags.indexOf(tag) < 0) {
            return false;
        }
    }

    return true;
}

function getRules() {
    var values  = config.get("extensions.automove.rules");
    var result = [];

    for(var i = 0; i < values.length; i++) {
        var path   = values[i].path;
        var filter = values[i].filter;

        if(filter === "pattern") {
            var pattern = values[i].data.pattern;
            var field   = values[i].data.field;

            result.push(new Rule(path, new PatternFilter(new RegExp(pattern), field)));
        } else if(filter === "tags") {
            result.push(new Rule(path, new TagsFilter(values[i].data)));
        }
    }

    return result;
}

function finished(rules, args) {
    var torrent = args.torrent;

    for(var i = 0; i < rules.length; i++) {
        var rule = rules[i];

        if(rule.filter.isMatch(torrent)) {
            logger.info("Moving torrent '" + torrent.infoHash + "' to " + rule.path);
            torrent.moveStorage(rule.path);
            break;
        }
    }
}

function load() {
    var enabled = config.getBoolean("extensions.automove.enabled");

    if(!enabled) {
        return;
    }
    
    var rules = getRules();
    logger.info("AutoMove enabled with " + rules.length + " rule(s).");

    session.on("torrent.finished", function(e) {
        finished(rules, e);
    });
}

exports.load = load;
