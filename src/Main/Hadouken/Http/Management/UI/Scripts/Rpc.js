var rpcRequestId = 0;

function rpc(pluginId, method, params, callback) {
    rpcRequestId += 1;

    var json = {
        id: rpcRequestId,
        jsonrpc: '2.0',
        method: method,
        params: params
    };

    $.post("/manage/jsonrpc", { pluginId: pluginId, json: JSON.stringify(json) }, function (result) {
        callback(JSON.parse(result));
    });
}