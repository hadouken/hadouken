angular.module('hadouken.bittorrent.controllers.torrentDetails', [
    'ui.bootstrap',
    'hadouken.jsonrpc'
])
.controller('BitTorrent.TorrentDetailsController', [
    '$scope', '$timeout', '$modalInstance', 'jsonrpc', 'torrent',
    function ($scope, $timeout, $modalInstance, jsonrpc, torrent) {
        var fileProgressTimer;

        $scope.torrent = torrent;

        function calculateProgress(total, done) {
            if (done === 0) return 0;

            return ((done / total) * 100.0).toFixed(1);
        }

        function updateFileProgress() {
            jsonrpc.request('torrents.getFileProgresses', {
                params: [torrent.InfoHash],
                success: function(data) {
                    for (var i = 0; i < data.result.length; i++) {
                        $scope.torrentFiles[i].Progress = calculateProgress($scope.torrentFiles[i].Size, data.result[i]);
                    }

                    fileProgressTimer = $timeout(updateFileProgress, 1000);
                }
            });
        }

        jsonrpc.request('torrents.getFileEntries', {
            params: [torrent.InfoHash],
            success: function (data) {
                $scope.torrentFiles = data.result;
                fileProgressTimer = $timeout(updateFileProgress);

                jsonrpc.request('torrents.getFilePriorities', {
                    params: [torrent.InfoHash],
                    success: function(d) {
                        for (var i = 0; i < d.result.length; i++) {
                            $scope.torrentFiles[i].Priority = d.result[i];
                        }
                    }
                });
            }
        });

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
            $timeout.cancel(fileProgressTimer);
        });
    }]);