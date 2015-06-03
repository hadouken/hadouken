var logger    = require("logger").get("events");
var callbacks = {};

exports.register = function(eventName, callback) {
    if(!callbacks[eventName]) {
        callbacks[eventName] = [];
    }

    callbacks[eventName].push(callback);
    return callbacks[eventName].length - 1;
}

exports.emitter = function(eventName, eventData) {
    var list = callbacks[eventName];

    if(!list) {
        return;
    }

    for(var i = 0; i < list.length; i++) {
        list[i](eventData);
    }
}
