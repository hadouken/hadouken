angular.module('hadouken.bittorrent.controllers.settings', [
    'hadouken.jsonrpc'
])
.controller('BitTorrent.SettingsController', [
    '$scope', 'jsonrpc', function ($scope, jsonrpc) {
        jsonrpc.request('torrents.config.get', {
            success: function(data) {
                $scope.config = data.result;
            }
        });

        $scope.save = function(config) {
            jsonrpc.request('torrents.config.set', {
                params: [config],
                success: function() {}
            });
        }
    }
]);