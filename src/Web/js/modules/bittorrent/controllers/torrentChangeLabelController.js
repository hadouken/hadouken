angular.module('hadouken.bittorrent.controllers.torrentChangeLabel', [
    'hadouken.jsonrpc'
])
.controller('BitTorrent.TorrentChangeLabelController', [
    '$scope', '$modalInstance', 'jsonrpc', 'infoHash',
    function ($scope, $modalInstance, jsonrpc, infoHash) {
        jsonrpc.request('torrents.getByInfoHash', {
            params: [infoHash],
            success: function(data) {
                $scope.torrent = data.result;
            }
        });

        $scope.changeLabel = function(newLabel) {
            jsonrpc.request('torrents.changeLabel', {
                params: [infoHash, newLabel],
                success: function() {
                    $modalInstance.close(true);
                },
                error: function() {
                    $modalInstance.close(false);
                }
            });
        }
    }]);