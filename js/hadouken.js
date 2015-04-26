(function(hadouken) {
    if (typeof String.prototype.endsWith !== "function") {
        String.prototype.endsWith = function(suffix) {
            return this.indexOf(suffix, this.length - suffix.length) !== -1;
        };
    }

    Duktape.modSearch = function(id, require, exports, module) {
        var nativeModules = [
            "bittorrent",
            "config",
            "core",
            "fs",
            "http",
            "logger"
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

    (function() {
        var logger = require("logger").get("hadouken");

        try {
            hadouken.emit = require("events").emitter;
            hadouken.rpc  = require("rpc").handler;

            require("plugins").load();
        } catch(e) {
            logger.error("Could not load Hadouken JS engine: " + e);
        }
    })();
});
