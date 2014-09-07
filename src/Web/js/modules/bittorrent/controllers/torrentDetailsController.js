angular.module('hadouken.bittorrent.controllers.torrentDetails', [
    'ui.bootstrap',
    'hadouken.jsonrpc'
])
.controller('BitTorrent.TorrentDetailsController', [
    '$scope', '$timeout', '$modalInstance', 'jsonrpc', 'torrent',
    function ($scope, $timeout, $modalInstance, jsonrpc, torrent) {
        var updateTimer;
        $scope.torrent = torrent;

        function update() {
            jsonrpc.request('torrents.getByInfoHash', {
                params: [torrent.InfoHash],
                success: function (data) {
                    // Update file progress
                    for(var i = 0; i < data.result.Files.length; i++) {
                        $scope.torrent.Files[i].Progress = data.result.Files[i].Progress;
                    }

                    // Update peers
                    $scope.torrent.Peers = data.result.Peers;

                    updateTimer = $timeout(update, 1000);
                }
            });
        }

        $scope.setPriority = function (fileIndex, priority) {
            $scope.torrentFiles[fileIndex].Priority = priority;

            jsonrpc.request('torrents.setFilePriority', {
                params: [torrent.InfoHash, fileIndex, priority]
            });
        };

        $scope.close = function() {
            $modalInstance.dismiss('close');
        };

        $scope.$on('$destroy', function() {
            $timeout.cancel(updateTimer);
        });

        updateTimer = $timeout(update);
    }]);