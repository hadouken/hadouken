var config = require("config");
var logger = require("logger").get("auth");

// Get username and password. We default to admin/admin, although
// this might be a bad idea. Maybe we should fail instead of
// defaulting.

var userName = config.getString("http.auth.basic.userName") || "admin";
var password = config.getString("http.auth.basic.password") || "admin";

function authenticator(header) {
    var parts = header.split(" ");

    if(parts.length === 0) {
        return false;
    }

    if("basic" !== String(parts[0]).toLowerCase()) {
        return false;
    }

    var data  = Duktape.dec("base64", String(parts[1])).toString();
    var combo = data.split(":");

    return (userName === combo[0] && password === combo[1]);
}


exports.authenticator = authenticator;
