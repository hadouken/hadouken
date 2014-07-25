angular.module('hadouken.settings.controllers.index', [])
    .controller('Settings.IndexController', [
        '$scope', '$http', 'jsonrpc',
        function ($scope, $http, jsonrpc) {
            jsonrpc.request('core.config.get', {
                success: function(data) {
                    $scope.settings = data.result;
                }
            });

            $scope.save = function() {
                jsonrpc.request('core.config.set', {
                    params: [$scope.settings],
                    success: function() {
                        console.log('saved');
                    },
                    error: function() {
                        throw new Error('Could not save settings.');
                    }
                });
            }
        }
    ]);