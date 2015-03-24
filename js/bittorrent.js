/*
Extends the native 'bittorrent' module with helper functions
for listening to events and other nice things.
*/

var callbacks = {};

function invoke(cb, data) {
    if(!cb) {
        return;
    }

    for(var i = 0; i < cb.length; i++) {
        cb[i](data);
    }
}

EventEmitter.register("torrent.added", function(torrent) {
    var cb = callbacks["torrent.added"];
    invoke(cb, torrent);
});

EventEmitter.register("torrent.finished", function(torrent) {
    var cb = callbacks["torrent.finished"];
    invoke(cb, torrent);
});

exports.session.on = function(eventName, callback) {
    if(!callbacks[eventName]) {
        callbacks[eventName] = [];
    }

    callbacks[eventName].push(callback);
}
