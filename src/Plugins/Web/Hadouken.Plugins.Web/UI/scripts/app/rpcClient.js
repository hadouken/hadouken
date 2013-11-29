define(['jquery'], function($) {
    function RpcClient() {
        this.requestId = 0;
        this.url = '/jsonrpc';
    }

    RpcClient.prototype.call = function(method, callback) {
        this.callParams(method, null, callback);
    };

    RpcClient.prototype.callParams = function(method, params, callback) {
        this.requestId += 1;

        var data = {
            id: this.requestId,
            jsonrpc: '2.0',
            method: method,
            params: params
        };

        var json = JSON.stringify(data);

        $.post(this.url, json, function(response) {
            if (typeof response.result === 'undefined') {
                throw new Error('error in jsonrpc response: ' + response.error);
            }
            
            callback(response.result);
        });
    };

    return RpcClient;
});