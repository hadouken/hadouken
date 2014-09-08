angular.module('hadouken.bittorrent.controllers.torrentMove', [
    'hadouken.jsonrpc'
])
.controller('BitTorrent.TorrentMoveController', [
    '$scope', '$modalInstance', 'jsonrpc', 'infoHash',
    function ($scope, $modalInstance, jsonrpc, infoHash) {
        jsonrpc.request('torrents.getByInfoHash', {
            params: [infoHash],
            success: function(data) {
                $scope.torrent = data.result;
            }
        });

        $scope.move = function(newPath, overwriteExisting) {
            console.log(overwriteExisting);
            
            jsonrpc.request('torrents.move', {
                params: [infoHash, newPath, overwriteExisting],
                success: function() {
                    $modalInstance.close(true);
                },
                error: function() {
                    $modalInstance.close(false);
                }
            });
        }
    }]);