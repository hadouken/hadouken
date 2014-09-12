angular.module('hadouken.bittorrent.controllers.torrentAdd', [
    'hadouken.jsonrpc',
    'hadouken.bittorrent.directives.fileread'
])
.controller('BitTorrent.TorrentAddController', [
    '$scope', '$modalInstance', 'jsonrpc',
    function ($scope, $modalInstance, jsonrpc) {
        $scope.add = function (file, savePath, label) {
            $scope.inProgress = true;

            var data = file.split(',')[1];

            var addParams = {
                label: label,
                savePath: savePath
            };

            jsonrpc.request('torrents.addFile', {
                params: [data, addParams],
                success: function() {
                    $modalInstance.close(true);
                },
                error: function() {
                    $modalInstance.close(false);
                }
            });
        }
    }]);