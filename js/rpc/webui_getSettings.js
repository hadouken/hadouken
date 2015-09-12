var config  = require("config");
var fs      = require("fs");
var session = require("bittorrent").session;

var ACCESS = {
    READ: "R",
    READWRITE: "Y",
    WRITEONLY: "W"
};

var TYPE = {
    NUMBER: 0,
    BOOLEAN: 1,
    STRING: 2,
    ARRAY: 3
};

function get(cfgKey, resKey, type, defaultValue, access) {
    var val = config.get(cfgKey);

    if(typeof val !== "undefined") {
        if(type !== TYPE.STRING) {
            val = JSON.stringify(val);
        }
    } else {
        val = defaultValue;
    }

    return [resKey,type,val,{access:access}];
}

exports.rpc = {
    name: "webui.getSettings",
    method: function() {
        /*
        the ui expects settings in the form
        [
            key,
            type,
            value,
            params
        ]

        key: string
        type: enum { 0=int, 1=boolean, 2=string}
        value: string (json encoded)
        params: object with extra properties, such as access
                which is R=read only, Y=read write, W=write only
        */

        var settings = session.getSettings();
        var savePath = config.getString("bittorrent.defaultSavePath") || ".";
        var absoluteSavePath = fs.makeAbsolute(savePath);

        if(savePath === "." && absoluteSavePath.endsWith(".")) {
            absoluteSavePath = absoluteSavePath.slice(0, -1);
        }

        var downloadDirectories = config.get("bittorrent.downloadDirectories") || [];

        return {
            settings: [
                [ "bind_port", TYPE.NUMBER, session.listenPort, ACCESS.READWRITE ],
                [ "conns_globally", TYPE.NUMBER, settings.getInt("connections_limit"), ACCESS.READWRITE ],
                [ "dht", TYPE.BOOLEAN, session.isDhtRunning.toString(), ACCESS.READWRITE ],
                [ "download_directories", TYPE.ARRAY, downloadDirectories, ACCESS.READWRITE ],
                [ "enable_bw_management", TYPE.BOOLEAN, settings.getInt("mixed_mode_algorithm") === 1 ? "true" : "false", ACCESS.READWRITE ],
                [ "max_dl_rate", TYPE.NUMBER, settings.getInt("download_rate_limit"), ACCESS.READWRITE ],
                [ "max_ul_rate", TYPE.NUMBER, settings.getInt("upload_rate_limit"), ACCESS.READWRITE ],
                [ "net.calc_overhead", TYPE.BOOLEAN, settings.getBool("rate_limit_ip_overhead").toString(), ACCESS.READWRITE ],
                [ "save_path", TYPE.STRING, absoluteSavePath, ACCESS.READWRITE ],
                get("bittorrent.lsd.enabled", "lsd", TYPE.BOOLEAN, "false", ACCESS.READWRITE),
                get("bittorrent.natpmp.enabled", "natpmp", TYPE.BOOLEAN, "false", ACCESS.READWRITE),
                get("bittorrent.upnp.enabled", "upnp", TYPE.BOOLEAN, "false", ACCESS.READWRITE),
                get("http.webui.cookie", "webui.cookie", TYPE.STRING, "{}", ACCESS.READWRITE),

                // Advanced settings
                [ "bt.allow_same_ip", TYPE.BOOLEAN, settings.getBool("allow_multiple_connections_per_ip").toString(), ACCESS.READWRITE ],
                [ "bt.anonymous_mode", TYPE.BOOLEAN, settings.getBool("anonymous_mode").toString(), ACCESS.READWRITE ],
                get("bittorrent.utp.enabled", "net.enable_utp", TYPE.BOOLEAN, "true", ACCESS.READWRITE)
            ]
        };
    }
};
