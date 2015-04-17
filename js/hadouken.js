(function(hadouken) {
    if (typeof String.prototype.endsWith !== "function") {
        String.prototype.endsWith = function(suffix) {
            return this.indexOf(suffix, this.length - suffix.length) !== -1;
        };
    }

    Duktape.modSearch = function(id, require, exports, module) {
        var nativeModules = [
            "bittorrent",
            "core",
            "fs"
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

    var rpcMethods = {};

    (function() {
        var fs = require("fs");
        var files = fs.getFiles("./rpc");

        for(var i = 0; i < files.length; i++) {
            var rpc = require(files[i]).rpc;

            if(rpc.name && rpc.method) {
                rpcMethods[rpc.name] = rpc.method;
            }
        }
    })();

    hadouken.rpc = function(request) {
        var method = rpcMethods[request.method];
        var response = {
            id: request.id,
            jsonrpc: "2.0"
        };

        if(method) {
            if(request.params instanceof Array) {
                response.result = method.apply(method, request.params);
            } else {
                response.result = method(request.params);
            }
        } else {
            response.error = {
                code: -32601,
                message: "Method not found",
                data: request.method
            };
        }

        return response;
    };
});
