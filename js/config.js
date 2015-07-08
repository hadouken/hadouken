var fs = require("fs");
var config = {};

if(fs.fileExists(__CONFIG__)) {
    var data = fs.readText(__CONFIG__);
    config = JSON.parse(data);
}

function findKey(key) {
    var parts = key.split(".");
    var val   = config;

    for(var i = 0; i < parts.length; i++) {
        val = val[parts[i]];

        if(typeof val === "undefined") {
            return;
        }
    }

    return val;
}

exports.obj = config;

exports.get = function(key) {
    return findKey(key);
}

exports.getBoolean = function(key) {
    return !!findKey(key);
}

exports.getNumber = function(key) {
    return findKey(key);
}

exports.getString = function(key) {
    return findKey(key);
}
