(function(hadouken) {
    if (typeof String.prototype.endsWith !== "function") {
        String.prototype.endsWith = function(suffix) {
            return this.indexOf(suffix, this.length - suffix.length) !== -1;
        };
    }

    Duktape.modSearch = function(id, require, exports, module) {
        var nativeModules = [
            "benc",
            "bittorrent",
            "config",
            "core",
            "fs",
            "http",
            "logger",
            "process"
        ];

        var found = false;
        var src;

        if(nativeModules.indexOf(id) > -1) {
            requireNative(id, require, exports, module);
            found = true;

            if(id === "fs" || id === "config") {
                return src;
            }
        }

        var config = require("config", require, exports, module);
        var fs     = require("fs", require, exports, module);

        if(!id.endsWith(".js")) {
            id = id + ".js";
        }

        var file = id;

        if(fs.isRelative(id)) {
            var scriptPath = config.getString("scripting.path");
            file = fs.combine(scriptPath, id);
        }

        src = fs.readText(file);

        if(typeof src === "string") {
            found = true;
        }

        if(!found) {
            throw new Error("Module not found: " + id);
        }

        return src;
    };

    (function() {
        try {
            var core = require("core");

            hadouken.emit = require("events").emitter;
            hadouken.load = core.load;
            hadouken.rpc  = require("rpc").handler;
            hadouken.unload = core.unload;
        } catch(e) {
            logger.error("Could not load Hadouken JS engine: " + e);
        }
    })();
});
