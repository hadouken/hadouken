/*
Event emitter. This should probably be moved elsewhere.
*/
EventEmitter = {};
EventEmitter.callbacks = {};

EventEmitter.emit = function(eventName, eventData) {
    print(JSON.stringify(arguments));

    var callbacks = EventEmitter.callbacks[eventName];

    if(!callbacks) {
        return;
    }

    for(var i = 0; i < callbacks.length; i++) {
        callbacks[i](eventData);
    }
};

EventEmitter.register = function(eventName, callback) {
    if(!EventEmitter.callbacks[eventName]) {
        EventEmitter.callbacks[eventName] = [];
    }

    EventEmitter.callbacks[eventName].push(callback);
};

/*
General nice-to-have functions.
*/
if (typeof String.prototype.endsWith !== "function") {
    String.prototype.endsWith = function(suffix) {
        return this.indexOf(suffix, this.length - suffix.length) !== -1;
    };
}

// Do our stuff
(function() {
    Duktape.modSearch = function(id, require, exports, module) {
        var nativeModules = [
            "bittorrent",
            "config",
            "fs"
        ];

        var found = false;
        var src;

        if(nativeModules.indexOf(id) > -1) {
            requireNative(id, require, exports, module);
            found = true;

            if(id === "fs") {
                return src;
            }
        }

        var fs = require("fs", require, exports, module);

        if(!id.endsWith(".js")) {
            id = id + ".js";
        }

        src = fs.readFile(id);

        if(typeof src === "string") {
            found = true;
        }

        if(!found) {
            throw new Error("Module not found: " + id);
        }

        return src;
    };

    /*
    Load all files in the "./plugins" folder. Each plugin file
    should export a "load" function and optionally an "unload"
    function.
    */
    function loadPlugins(files) {
        for(var i = 0; i < files.length; i++) {
            var plugin = require(files[i]);

            if(!plugin.load || typeof plugin.load !== "function") {
                alert(files[i] + " does not export a 'load' function.");
                continue;
            }

            plugin.load();
        }
    }

    var fs = require("fs");
    var pluginFiles = fs.getFiles("./plugins");

    if(pluginFiles && pluginFiles.length > 0) {
        loadPlugins(pluginFiles);
    } else {
        alert("No plugin files found.");
    }

})();
