var config = require("config");
var logger = require("logger").get("rpc");
var fs     = require("fs");

/*
Object hash containing all RPC methods.
*/
var methods = {};

(function() {
    function introspect() {
        return Object.keys(methods);
    }

    methods["core.introspect"] = introspect;
})();

(function() {
    function multiCall(params) {
        var methodNames = Object.keys(params);
        var result = {};

        for(var i = 0; i < methodNames.length; i++) {
            var name = methodNames[i];
            var method = methods[name];

            result[name] = invoke(method, params[name]);
        }

        return result;
    }

    methods["core.multiCall"] = multiCall;
})();

(function() {
    var rpcFiles   = fs.combine(__ROOT__, "rpc");

    var files = fs.getFiles(rpcFiles);

    for(var i = 0; i < files.length; i++) {
        var rpc = require(files[i]).rpc;

        if(rpc.name && rpc.method) {
            methods[rpc.name] = rpc.method;
        }
    }
})();

logger.info("Found " + Object.keys(methods).length + " RPC method(s).");

function invoke(method, params) {
    var fn = null;

    if(params instanceof Array) {
        fn = function() { return method.apply(method, params); };
    } else {
        fn = function() { return method(params); };
    }

    return fn();
}

function handleRequest(request) {
    var method = methods[request.method];
    var response = {
        id: request.id,
        jsonrpc: "2.0"
    };

    if(method) {
        try {
            response.result = invoke(method, request.params);
        } catch(e) {
            response.result = undefined;
            response.error = {
                code: -32000,
                message: "Internal server error",
                data: {
                    name: e.name,
                    message: e.message
                }
            };

            logger.error("Error when executing RPC method '" + request.method + "': " + e);
        }
    } else {
        response.error = {
            code: -32601,
            message: "Method not found",
            data: request.method
        };

        logger.error("Method '" + request.method + "' not found.");
    }

    return response;
}

exports.handler = handleRequest;
