var events    = require("events");
var callbacks = [];

events.register("tick", function() {
    for(var i = 0; i < callbacks.length; i++) {
        callbacks[i]();
    }
});

exports.tick = function(callback) {
    callbacks.push(callback);
};
