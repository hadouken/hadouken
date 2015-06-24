var config = require("config");
var logger = require("logger").get("auth");

// How to authenticate
var authType = config.getString("http.auth.type") || "none";
    authType = authType.toLowerCase();

if(authType === "none") {
    logger.warn("No HTTP authentication configured.");
}

function authenticator(header) {
    if(authType === "none") {
        return true;
    }

    var parts = header.split(" ");

    if(parts.length === 0) {
        return false;
    }

    if(authType !== String(parts[0]).toLowerCase()) {
        return false;
    }

    if(authType === "token") {
        var token = config.getString("http.auth.token") || "";

        if(token === "") {
            logger.warn("No token for HTTP token auth specified.");
            return false;
        }

        return token === parts[1];
    }

    if(authType === "basic") {
        var data  = Duktape.dec("base64", String(parts[1])).toString();
        var combo = data.split(":");

        var user = config.getString("http.auth.basic.userName") || "";
        var pass = config.getString("http.auth.basic.password") || "";

        if(user === "" || pass === "") {
            logger.warn("No username/password for HTTP basic auth specified.");
            return false;
        }

        return (user === combo[0] && pass === combo[1]);
    }

    return false;
}


exports.authenticator = authenticator;
