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

            if(id === "fs") {
                return src;
            }
        }

        var fs = require("fs", require, exports, module);

        if(!id.endsWith(".js")) {
            id = id + ".js";
        }

        var file = id;

        if(fs.isRelative(id)) {
            file = fs.combine(__ROOT__, id);
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
        var logger = require("logger").get("loader");

        try {
            var core = require("core");

            hadouken.authenticator = require("auth").authenticator;
            hadouken.emit = require("events").emitter;
            hadouken.load = core.load;
            hadouken.rpc  = require("rpc").handler;
            hadouken.unload = core.unload;
        } catch(e) {
            logger.error("Could not load Hadouken JS engine: " + e);
        }
    })();
});
