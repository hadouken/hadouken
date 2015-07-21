var config  = require("config");
var session = require("bittorrent").session;

var ACCESS = {
    READ: "R",
    READWRITE: "Y",
    WRITEONLY: "W"
};

var TYPE = {
    NUMBER: 0,
    BOOLEAN: 1,
    STRING: 2
};

function get(cfgKey, resKey, type, defaultValue, access) {
    var val = config.get(cfgKey);

    if(val) {
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

        return {
            settings: [
                [ "bind_port", TYPE.NUMBER, session.listenPort, ACCESS.READWRITE ],
                [ "conns_globally", TYPE.NUMBER, settings.connectionsLimit, ACCESS.READWRITE ],
                [ "dht", TYPE.BOOLEAN, session.isDhtRunning.toString(), ACCESS.READWRITE ],
                [ "enable_bw_management", TYPE.BOOLEAN, settings.mixedModeAlgorithm === 1 ? "true" : "false", ACCESS.READWRITE ],
                [ "max_dl_rate", TYPE.NUMBER, settings.downloadRateLimit, ACCESS.READWRITE ],
                [ "max_ul_rate", TYPE.NUMBER, settings.uploadRateLimit, ACCESS.READWRITE ],
                [ "net.calc_overhead", TYPE.BOOLEAN, settings.rateLimitIpOverhead.toString(), ACCESS.READWRITE ],
                [ "net.ratelimit_utp", TYPE.BOOLEAN, settings.rateLimitUtp.toString(), ACCESS.READWRITE ],
                get("bittorrent.lsd.enabled", "lsd", TYPE.BOOLEAN, "false", ACCESS.READWRITE),
                get("bittorrent.natpmp.enabled", "natpmp", TYPE.BOOLEAN, "false", ACCESS.READWRITE),
                get("bittorrent.upnp.enabled", "upnp", TYPE.BOOLEAN, "false", ACCESS.READWRITE),
                get("http.webui.cookie", "webui.cookie", TYPE.STRING, "{}", ACCESS.READWRITE)
            ]
        };
    }
};
