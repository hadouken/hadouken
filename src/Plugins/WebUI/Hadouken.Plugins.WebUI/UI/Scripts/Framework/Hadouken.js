var Hadouken = {
    init: function () {
        var rpcClient = new JsonRpcClient();
        
        rpcClient.call("plugins.list", null, function(pluginList) {
            for (var i = 0; i < pluginList.length; i++) {
                console.log(pluginList[i].name);
            }
        });
    }
};