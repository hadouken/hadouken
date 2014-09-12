angular.module('hadouken.bittorrent.controllers.torrentList', [
    'ui.bootstrap',
    'hadouken.jsonrpc'
])
.filter('torrentProgress', function () {
    return function (torrent) {
        if (torrent.State === 'CheckingFiles'
            || torrent.state === 'CheckingResumeData'
            || torrent.State === 'Downloading') {
            var progress = (torrent.Progress * 100) | 0;
            return '(' + progress + '%)';
        }

        return '';
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
.controller('BitTorrent.TorrentListController', [
    '$scope', '$timeout', '$modal', 'jsonrpc',
    function ($scope, $timeout, $modal, jsonrpc) {
        $scope.torrents = {};

        $scope.showAdd = function() {
            $modal.open({
                controller: 'BitTorrent.TorrentAddController',
                templateUrl: 'views/bittorrent/add-torrents.html'
            });
        };

        $scope.showDetails = function(infoHash) {
            $modal.open({
                controller: 'BitTorrent.TorrentDetailsController',
                resolve: {
                    torrent: function() {
                        return $scope.torrents[infoHash];
                    }
                },
                templateUrl: 'views/bittorrent/details.html'
            });
        };

        $scope.resume = function(infoHash) {
            jsonrpc.request('torrents.resume', {
                params: [infoHash],
                success: function() {}
            });
        };

        $scope.pause = function(infoHash) {
            jsonrpc.request('torrents.pause', {
                params: [infoHash],
                success: function () { }
            });
        };

        $scope.move = function(infoHash) {
            $modal.open({
                controller: 'BitTorrent.TorrentMoveController',
                resolve: {
                    infoHash: function() {
                        return infoHash;
                    }
                },
                templateUrl: 'views/bittorrent/move-torrent.html'
            });
        };

        $scope.changeLabel = function(infoHash) {
            $modal.open({
                controller: 'BitTorrent.TorrentChangeLabelController',
                resolve: {
                    infoHash: function() {
                        return infoHash;
                    }
                },
                templateUrl: 'views/bittorrent/change-label.html'
            });
        };

        $scope.remove = function(infoHash) {
            jsonrpc.request('torrents.remove', {
                params: [infoHash, false],
                success: function() {}
            });

            delete $scope.torrents[infoHash];
        };

        function update() {
            jsonrpc.request('torrents.getAll', {
                success: function (data) {
                    for (var i = 0; i < data.result.length; i++) {
                        var torrent = data.result[i];
                        $scope.torrents[torrent.InfoHash] = torrent;
                    }

                    if (data.result.length) {
                        var currentIdList = data.result.map(function(x) { return x.InfoHash; });
                        var selfIds = Object.keys($scope.torrents);

                        for (var i = 0; i < selfIds.length; i++) {
                            if (currentIdList.indexOf(selfIds[i]) < 0) {
                                delete $scope.torrents[selfIds[i]];
                            }
                        }
                    }

                    timer = $timeout(update, 1000);
                }
            });
        }
        var timer = $timeout(update, 0);

        $scope.$on('$destroy', function() {
            $timeout.cancel(timer);
        });
    }
]);