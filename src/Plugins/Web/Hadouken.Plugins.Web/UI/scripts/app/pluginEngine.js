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
        var keys = Object.keys(this.plugins);

        var asyncLoop = function(o) {
            var i = -1;

            var loop = function() {
                i++;

                if (i == o.length) {
                    o.callback();
                    return;
                }

                o.loopTarget(loop, i);
            };

            loop();
        };

        asyncLoop({
            length: keys.length,
            loopTarget: function(loopCallback, index) {
                var plugin = that.plugins[keys[index]];
                
                if (plugin.name === 'core.web') {
                    loopCallback();
                    return;
                }
                
                $.ajax({
                    url: '/plugins/' + plugin.name + '/js/factory.js',
                    success: function (js) {
                        var funcFactory = new Function('return ' + js);
                        var func = funcFactory();

                        func(function (instance) {
                            that.plugins[plugin.name].instance = instance;
                            that.plugins[plugin.name].instance.load();

                            loopCallback();
                        });
                    },
                    error: loopCallback
                });
            },
            callback: function() {
                callback();
            }
        });
    };

    return PluginEngine;
});