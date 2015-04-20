var fs = require("fs");
var methods = {};

(function() {
    var files = fs.getFiles("./rpc");

    for(var i = 0; i < files.length; i++) {
        var rpc = require(files[i]).rpc;

        if(rpc.name && rpc.method) {
            methods[rpc.name] = rpc.method;
        }
    }
})();

function handleRequest(request) {
    var method = methods[request.method];
    var response = {
        id: request.id,
        jsonrpc: "2.0"
    };

    if(method) {
        var fn = null;

        if(request.params instanceof Array) {
            fn = function() { return method.apply(method, request.params); };
        } else {
            fn = function() { return method(request.params); };
        }

        try {
            response.result = fn();
        } catch(e) {
            response.result = undefined;
            response.error = {
                code: -32000,
                message: "Internal server error",
                data: e
            };
        }
    } else {
        response.error = {
            code: -32601,
            message: "Method not found",
            data: request.method
        };
    }

    return response;
}

exports.handler = handleRequest;
