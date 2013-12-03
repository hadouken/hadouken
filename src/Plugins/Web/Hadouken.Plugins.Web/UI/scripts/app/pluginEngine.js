define(['rpcClient'], function (RpcClient) {
    function PluginEngine() {
        this.plugins = {};
        this.rpc = new RpcClient();
    }

    PluginEngine.getInstance = function () {
        if (this._instance === undefined) {
            this._instance = new PluginEngine();
        }
        
        return this._instance;
    };

    PluginEngine.prototype.load = function (callback) {
        var that = this;
        
        this.rpc.call('plugins.list', function(plugins) {
            for (var i = 0; i < plugins.length; i++) {
                var plugin = plugins[i];
                that.plugins[plugin.name] = plugin;
            }

            that.loadPluginScripts(callback);
        });
    };

    PluginEngine.prototype.loadPluginScripts = function (callback) {
        var that = this;
        
        var files = 0;
        var requestCallback = function() {
            files += 1;
            
            if (files >= Object.keys(that.plugins).length) {
                callback();
            }
        };

        var keys = Object.keys(this.plugins);
        
        for (var i = 0; i < keys.length; i++) {
            var plugin = this.plugins[keys[i]];
            
            if (plugin.name === 'core.web') {
                requestCallback();
                continue;
            }

            $.ajax({
                url: '/plugins/' + plugin.name + '/js/factory.js',
                success: function(js) {
                    var funcFactory = new Function('return ' + js);
                    var func = funcFactory();

                    func(function(instance) {
                        plugin.instance = instance;
                        plugin.instance.load();

                        requestCallback();
                    });
                },
                error: requestCallback
            });
        }
    };

    return PluginEngine;
});