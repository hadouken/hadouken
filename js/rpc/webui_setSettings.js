var config  = require("config");
var session = require("bittorrent").session; 

function set(settings, key, value) {
    switch(key) {
        case "bind_port":
            var port = parseInt(value, 10);
            if(isNaN(port) || port < 0 || port > 65535) {
                "Invalid port";
            }
            config.set("bittorrent.listenPort", port);
            settings.setStr("listen_interfaces", "0.0.0.0" + port);
            break;

        case "bt.anonymous_mode":
            value = !!(value);
            config.set("bittorrent.anonymousMode", value);
            settings.setBool("anonymous_mode", value);
            break;

        case "bt.allow_same_ip":
            value = !!(value);
            config.set("bittorrent.allowMultipleConnectionsPerIp", value);
            settings.setBool("allow_multiple_connections_per_ip", value);
            break;

        case "conns_globally":
            value = parseInt(value, 10);
            if(!isNaN(value)) {
                config.set("bittorrent.connectionsLimit", value);
                settings.setInt("connections_limit", value);
            }
            break;

        case "dht":
            value = !!(value);
            config.set("bittorrent.dht.enabled", value);
            settings.setBool("enable_dht", value);
            break;

        case "download_directories":
            if(Array.isArray(value)) {
                config.set("bittorrent.downloadDirectories", value);
            }
            break;

        case "enable_bw_management":
            value = (!!(value) ? 1 : 0);
            config.set("bittorrent.mixedModeAlgorithm", value);
            settings.setBool("mixed_mode_algorithm", value);
            break;

        case "lsd":
            value = !!(value);
            config.set("bittorrent.lsd.enabled", value);
            settings.setBool("enable_lsd", value);
            break;

        case "max_dl_rate":
            var rate = parseInt(value, 10);
            if(!isNaN(rate)) {
                config.set("bittorrent.downloadRateLimit", rate);
                settings.setInt("download_rate_limit", rate);
            }
            break;

        case "max_ul_rate":
            var rate = parseInt(value, 10);
            if(!isNaN(rate)) {
                config.set("bittorrent.uploadRateLimit", rate);
                settings.setInt("upload_rate_limit", rate);
            }
            break;

        case "net.calc_overhead":
            value = !!(value);
            config.set("bittorrent.rateLimitIpOverhead", value);
            settings.setInt("rate_limit_ip_overhead", value);
            break;

        case "net.enable_utp":
            value = !!(value);
            config.set("bittorrent.utp.enabled", value);
            settings.setBool("enable_incoming_utp", value);
            settings.setBool("enable_outgoing_utp", value);
            break;

        case "natpmp":
            value = !!(value);
            config.set("bittorrent.natpmp.enabled", value);
            settings.setBool("enable_natpmp", value);
            break;

        case "upnp":
            value = !!(value);
            config.set("bittorrent.upnp.enabled", value);
            settings.setBool("enable_upnp", value);
            break;

        case "save_path":
            config.set("bittorrent.defaultSavePath", value);
            break;

        case "webui.cookie":
            config.set("http.webui.cookie", value);
            break;
    }
}

exports.rpc = {
    name: "webui.setSettings",
    method: function(settings) {
        var keys = Object.keys(settings);
        var sessionSettings = session.getSettings();

        for(var i = 0; i < keys.length; i++) {
            var key = keys[i];
            set(sessionSettings, key, settings[key]);
        }

        session.applySettings(sessionSettings);
    }
};
