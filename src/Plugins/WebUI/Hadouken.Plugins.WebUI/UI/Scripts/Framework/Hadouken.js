var Hadouken = {
    init: function () {
        var rpcClient = new JsonRpcClient();
        
        rpcClient.call("plugins.list", null, function(pluginList) {
            for (var i = 0; i < pluginList.length; i++) {
                // Load plugins main.js files
                var plugin = pluginList[i];
                
                try {
                    Asset.javascript('plugins/' + plugin.name + '/scripts/main.js');
                } catch(e) {
                    console.log(e);
                }
            }
        });
    }
};