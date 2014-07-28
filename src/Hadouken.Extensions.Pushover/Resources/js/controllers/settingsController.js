angular.module('pushover.controllers.settings', [
    'hadouken.jsonrpc'
])
.controller('Pushover.SettingsController', [
    '$scope', 'jsonrpc', function ($scope, jsonrpc) {
        jsonrpc.request('pushover.config.get', {
            success: function (data) {
                $scope.config = data.result;
            }
        });

        $scope.save = function (config) {
            jsonrpc.request('pushover.config.set', {
                params: [config],
                success: function () { }
            });
        }
    }
]);