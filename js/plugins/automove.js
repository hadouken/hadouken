var config  = require("config");
var logger  = require("logger").get("plugins.automove");
var session = require("bittorrent").session;

// Configuration
var rules   = [];

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
    var torrentTags = torrent.metadata.get("tags");
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
    var key = "extensions.automove.rules";
    var result = [];

    for(var i = 0; i < Number.MAX_VALUE; i++) {
        var query = key + "[" + i + "]";

        if(config.has(query)) {
            var path   = config.getString(query + ".path");
            var filter = config.getString(query + ".filter");

            if(filter === "pattern") {
                var pattern = config.getString(query + ".data.pattern");
                var field   = config.getString(query + ".data.field");

                result.push(new Rule(path, new PatternFilter(new RegExp(pattern), field)));
            } else if(filter === "tags") {
                var tags = [];

                for(var j = 0; j < Number.MAX_VALUE; j++) {
                    var tagQuery = query + ".data[" + j + "]";

                    if(config.has(tagQuery)) {
                        tags.push(config.getString(tagQuery));
                    } else {
                        break;
                    }
                }

                result.push(new Rule(path, new TagsFilter(tags)));
            }
        } else {
            break;
        }
    }

    return result;
}

function finished(torrent) {
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
    
    rules = getRules();
    session.on("torrent.finished", finished);
}

exports.load = load;
