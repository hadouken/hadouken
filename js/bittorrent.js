var events = require("events");

function on(eventName, callback) {
    return events.register("bt." + eventName, callback);
}

exports.session.on = on;
