var JsonRpcClient = new Class({
    requestId: 0,
    call: function (methodName, params, callback) {
        this.requestId++;
        
        var data = JSON.encode({
            id: this.requestId,
            jsonrpc: '2.0',
            method: methodName,
            params: params
        });
        
        new Request.JSON({
            url: '/jsonrpc',
            urlEncoded: false,
            data: data,
            onSuccess: function(response) {
                callback(response.result);
            }
        }).send();
    }
});