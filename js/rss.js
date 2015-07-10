var bt       = require("bittorrent");
var config   = require("config");
var fs       = require("fs");
var logger   = require("logger").get("rss");
var session  = bt.session;

// Constants
var STATE_UPDATING = 0;
var STATE_UPDATED  = 1;
var STATE_ERROR    = 2;

// Our managed feeds
var feeds = [];

// Prototypes for feeds and filters
function Feed(url, filter, options) {
    this.url = url;
    this.history = [];
    this.options = options || {};

    if(typeof filter === "string" && filter === "*") {
        this.filter = new CatchAllFilter();
    } else if(filter instanceof Array) {
        var type = filter[0];

        if(type === "regex") {
            this.filter = new RegExpFilter(filter[1], filter[2]);
        } else {
            this.filter = new Filter();
        }
    } else {
        this.filter = new Filter();
    }
}

Feed.find = function(url) {
    for(var i = 0; i < feeds.length; i++) {
        if(feeds[i].url === url) {
            return feeds[i];
        }
    }
}

function Filter() {}
Filter.prototype.isMatch = function() { return false; };

function CatchAllFilter() {}
CatchAllFilter.prototype.isMatch = function() { return true; }

function RegExpFilter(include, exclude) {
    this.include = new RegExp(include);

    if(typeof exclude !== "undefined") {
        this.exclude = new RegExp(exclude);
    }
}

RegExpFilter.prototype.isMatch = function(input) {
    if(typeof this.exclude !== "undefined") {
        return this.include.test(input) && !this.exclude.test(input);
    }

    return this.include.test(input);
}

function download(item, options) {
    var p  = bt.AddTorrentParams.getDefault();
    p.url  = item.url;

    if(options.savePath) {
        p.savePath = options.savePath;
    }

    session.addTorrent(p);
}

function onRss(arg) {
    if(arg.state === STATE_ERROR) {
        logger.error("Error when updating feed '" + arg.url + "'.");
        return;
    }

    if(arg.state === STATE_UPDATING) {
        return;
    }

    // Every time a feed is updated, loop through the items and
    // match each item against the filter specified for this feed.

    var status = arg.feed.getStatus();
    var feed   = Feed.find(status.url);

    if(!feed) {
        return;
    }

    var items  = status.getItems();

    for(var i = 0; i < items.length; i++) {
        var item = items[i];

        // Skip item if it exists in feed history.
        if(feed.history.indexOf(item.uuid) > -1) {
            continue;
        }

        // Skip item if the filter does not match.
        if(!feed.filter.isMatch(item.title)) {
            continue;
        }

        if(feed.options.dryRun) {
            logger.debug("(RSS DRY-RUN): Adding torrent " + item.title);
        } else {
            // Push the uuid to the feed history and
            // download the item.
            feed.history.push(item.uuid);
            download(item, feed.options);
        }
    }
}

function loadState() {
    var statePath = config.getString("bittorrent.statePath") || "state";
    var stateFile = fs.combine(statePath, ".rss_state");

    if(!fs.fileExists(stateFile)) {
        logger.debug("No previous RSS state to load.");
        return;
    }

    var contents = fs.readText(stateFile);
    var state    = JSON.parse(contents);

    return state;
}

function load() {
    session.on("rss", onRss);

    var configuredFeeds = config.get("feeds") || [];
    var state           = loadState() || {};

    for(var i = 0; i < configuredFeeds.length; i++) {
        var feed          = new bt.FeedSettings();
        feed.url          = configuredFeeds[i].url;
        feed.ttl          = configuredFeeds[i].ttl || 30;
        feed.autoDownload = false;

        var f = new Feed(feed.url, configuredFeeds[i].filter, configuredFeeds[i].options);

        if(state[f.url]) {
            f.history = state[f.url] || [];
        }

        feeds.push(f);
        session.addFeed(feed);
    }
}

function saveState() {
    var statePath = config.getString("bittorrent.statePath") || "state";
    var stateFile = fs.combine(statePath, ".rss_state");

    var state = {};

    for(var i = 0; i < feeds.length; i++) {
        state[feeds[i].url] = feeds[i].history;
    }

    fs.writeText(stateFile, JSON.stringify(state, 2, ' '));
    logger.info("RSS state saved to " + stateFile);
}

function unload() {
    saveState();
}

exports.load = load;
exports.unload = unload;
