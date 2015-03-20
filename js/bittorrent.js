/*
Extends the native 'bittorrent' module with helper functions
for listening to events and other nice things.
*/

var callbacks = {};

exports.session.on = function(eventName, callback) {
    if(!callbacks[eventName]) {
        callbacks[eventName] = [];
    }

    callbacks[eventName].push(callback);
}
