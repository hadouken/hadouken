angular.module('hadouken.tools.controllers.jsonrpc', [])
    .controller('Tools.JsonRpcController', [
        '$scope', 'jsonrpc',
        function ($scope, jsonrpc) {
            $scope.json = JSON.stringify({
              "method": "core.plugins.list",
              "params": null
            }, null, 2);

            $scope.send = function(json) {
                var start = performance.now();

                var d = JSON.parse(json);
                jsonrpc.request(d.method, {
                    params: d.params,
                    success: function(data) {
                        $scope.result = JSON.stringify(data, null, 2);
                        $scope.responseTime = performance.now() - start;
                    },
                    error: function(data) {
                        $scope.result = JSON.stringify(data, null, 2);
                        $scope.responseTime = performance.now() - start;
                    }
                });
            };
        }
    ]);