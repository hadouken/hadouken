var fs     = require("fs");
var logger = require("logger").get("plugins");

function load() {
    var plugins = fs.getFiles("./plugins");

    for(var i = 0; i < plugins.length; i++) {
        try {
            require(plugins[i]).load();
            logger.info("Loaded plugin '" + plugins[i] + "'");
        } catch(e) {
            logger.error("Could not load plugin '" + plugins[i] + "': " + e.toString());
        }
    }
}

exports.load = load;
