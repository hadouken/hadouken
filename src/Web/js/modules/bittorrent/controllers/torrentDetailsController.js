angular.module('hadouken.bittorrent.controllers.torrentDetails', [
    'ui.bootstrap',
    'hadouken.jsonrpc'
])
.filter('fileProgress', function() {
    return function(progress) {
        return (progress * 100) | 0;
    }
})
.filter('ratio', function() {
    return function(torrent) {
        var dl = torrent.TotalDownloadedBytes;
        var ul = torrent.TotalUploadedBytes;

        if(ul > 0 && dl <= 0) {
            return '∞';
        }

        if(dl <= 0) {
            return (0).toFixed(2);
        }

        return (ul / dl).toFixed(2);
    }
})
.filter('speed', function () {
    return function (bytes, precision) {
        if (isNaN(parseFloat(bytes)) || !isFinite(bytes) || bytes <= 1024) return '-';
        if (typeof precision === 'undefined') precision = 1;
        var units = ['B/s', 'KiB/s', 'MiB/s', 'GiB/s', 'TiB/s', 'PiB/s'],
            number = Math.floor(Math.log(bytes) / Math.log(1024));
        return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
    }
})
.controller('BitTorrent.TorrentDetailsController', [
    '$scope', '$timeout', '$modalInstance', 'jsonrpc', 'torrent',
    function ($scope, $timeout, $modalInstance, jsonrpc, torrent) {
        var updateTimer;
        $scope.peers = {};
        $scope.peersCount = 0;
        $scope.priorities = [];
        $scope.progress = [];
        $scope.settings = {};
        $scope.torrent = torrent;

        function getFilePriorities(cb) {
            jsonrpc.request('torrents.getFilePriorities', {
                params: [torrent.InfoHash],
                success: function(d) {
                    $scope.priorities = d.result;
                    cb();
                }
            });
        }

        function getSettings(cb) {
            jsonrpc.request('torrents.getSettings', {
                params: [torrent.InfoHash],
                success: function(d) {
                    $scope.settings = d.result;
                    cb();
                }
            });
        }

        function updateFileProgress(cb) {
            jsonrpc.request('torrents.getFileProgress', {
                params: [torrent.InfoHash],
                success: function (data) {
                    $scope.progress = data.result;
                    cb();
                }
            });
        }

        function updatePeers(cb) {
            jsonrpc.request('torrents.getPeers', {
                params: [torrent.InfoHash],
                success: function(d) {
                    $scope.peersCount = d.result.length;

                    for(var i = 0; i < d.result.length; i++) {
                        var peer = d.result[i];
                        $scope.peers[peer.IP] = peer;
                    }

                    var currentIps = d.result.map(function(x) { return x.IP; });

                    for(var key in $scope.peers) {
                        if(currentIps.indexOf(key) < 0) {
                            delete $scope.peers[key];
                        }
                    }

                    cb();
                }
            });
        }

        function update() {
            updateFileProgress(function() {
                updatePeers(function() {
                    updateTimer = $timeout(update, 1000);
                });
            });
        }

        $scope.setPriority = function (fileIndex, priority) {
            jsonrpc.request('torrents.setFilePriority', {
                params: [torrent.InfoHash, fileIndex, priority],
                success: function() {
                    $scope.priorities[fileIndex] = priority;
                }
            });
        };

        $scope.saveSettings = function() {
            $scope.busy = true;

            jsonrpc.request('torrents.setSettings', {
                params: [torrent.InfoHash, $scope.settings],
                success: function() {
                    $scope.busy = false;
                }
            });
        };

        $scope.close = function() {
            $modalInstance.dismiss('close');
        };

        $scope.$on('$destroy', function() {
            $timeout.cancel(updateTimer);
        });

        jsonrpc.request('torrents.getFiles', {
            params: [torrent.InfoHash],
            success: function(data) {
                $scope.files = data.result;

                getSettings(function() {
                    getFilePriorities(function() {
                        updateTimer = $timeout(update);
                    });
                });
            }
        });
    }]);