angular.module('hadouken.bittorrent.controllers.torrentList', [
    'ui.bootstrap',
    'hadouken.events',
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
.filter('prettyQueue', function() {
    return function(queuePosition) {
        if(queuePosition < 0) {
            return '';
        }

        return queuePosition + 1;
    }
})
.filter('sort', function() {
    function compareByName(a, b) { an = a.Name.toUpperCase(); bn = b.Name.toUpperCase(); if(an > bn) { return 1; } if(an < bn) { return -1; } return 0; }
    function compareByProgress(a, b) { if(a.Progress > b.Progress) { return 1; } if(a.Progress < b.Progress) { return -1; } return 0; }
    function compareByQueuePosition(a, b, desc) {
        var ap = a.QueuePosition;
        var bp = b.QueuePosition;

        // The following two assignments makes sure the -1 (seeding) always comes last
        if(!desc && ap === -1) { ap = 1000000; }
        if(!desc && bp === -1) { bp = 1000000; }

        if(ap > bp) { return 1; }
        if(ap < bp) { return -1; }

        return 0;
    }

    var comparers = {};
    comparers['Name'] = compareByName;
    comparers['Progress'] = compareByProgress;
    comparers['QueuePosition'] = compareByQueuePosition;

    return function(array, field, direction) {
        var comparer = comparers[field];
        var sorted = [];

        angular.forEach(array, function(item) { sorted.push(item); });
        
        if(direction === 'desc') {
            sorted.sort(function(a, b) {
                return comparer(b, a, true);
            });
        } else {
            sorted.sort(comparer);
        }

        return sorted;
    }
})
.controller('BitTorrent.TorrentListController', [
    '$document', '$scope', '$timeout', '$modal', 'eventListener', 'jsonrpc',
    function ($document, $scope, $timeout, $modal, eventListener, jsonrpc) {
        $scope.sortField = readCookie('sortField') || 'Name';
        $scope.sortDirection = readCookie('sortDirection') || 'asc';
        $scope.torrents = [];

        $scope.sort = function(field, dir) {
            $scope.sortField = field;
            $scope.sortDirection = dir;

            createCookie('sortField', field, 30);
            createCookie('sortDirection', dir, 30);
        };

        $scope.showAdd = function() {
            $modal.open({
                controller: 'BitTorrent.TorrentAddController',
                templateUrl: 'views/bittorrent/add-torrents.html'
            });
        };

        $scope.showDetails = function(infoHash) {
            var index = getIndex(infoHash);

            $modal.open({
                controller: 'BitTorrent.TorrentDetailsController',
                resolve: {
                    torrent: function() {
                        return $scope.torrents[index];
                    }
                },
                templateUrl: 'views/bittorrent/details.html'
            });
        };

        $scope.resume = function(infoHash) {
            notify('torrents.resume', infoHash);
        };

        $scope.pause = function(infoHash) {
            notify('torrents.pause', infoHash);
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
        };

        $scope.queuePosUp = function(infoHash) {
            notify('torrents.queue.up', infoHash);
        };

        $scope.queuePosDown = function(infoHash) {
            notify('torrents.queue.down', infoHash);
        };

        $scope.queuePosTop = function(infoHash) {
            notify('torrents.queue.top', infoHash);
        };

        $scope.queuePosBottom = function(infoHash) {
            notify('torrents.queue.bottom', infoHash);
        };

        // Create cookie
        function createCookie(name, value, days) {
            var expires;
            if (days) {
                var date = new Date();
                date.setTime(date.getTime()+(days*24*60*60*1000));
                expires = "; expires="+date.toGMTString();
            }
            else {
                expires = "";
            }
            $document[0].cookie = name+"="+value+expires+"; path=/";
        }

        // Read cookie
        function readCookie(name) {
            var nameEQ = name + "=";
            var ca = $document[0].cookie.split(';');
            for(var i=0;i < ca.length;i++) {
                var c = ca[i];
                while (c.charAt(0) === ' ') {
                    c = c.substring(1,c.length);
                }
                if (c.indexOf(nameEQ) === 0) {
                    return c.substring(nameEQ.length,c.length);
                }
            }
            return null;
        }

        function getIndex(infoHash) {
            for(var i = 0; i < $scope.torrents.length; i++) {
                if(infoHash === $scope.torrents[i].InfoHash) {
                    return i;
                }
            }

            return -1;
        }

        function notify(method, infoHash) {
            jsonrpc.request(method, {
                params: [infoHash],
                success: function() {}
            });
        }

        function updateTorrent(oldTorrent, newTorrent) {
            oldTorrent.DownloadSpeed = newTorrent.DownloadSpeed;
            oldTorrent.Paused = newTorrent.Paused;
            oldTorrent.Progress = newTorrent.Progress;
            oldTorrent.QueuePosition = newTorrent.QueuePosition;
            oldTorrent.SavePath = newTorrent.SavePath;
            oldTorrent.Size = newTorrent.Size;
            oldTorrent.State = newTorrent.State;
            oldTorrent.UploadSpeed = newTorrent.UploadSpeed;
        }

        function getTorrents(callback) {
            jsonrpc.request('torrents.getAll', {
                success: function(data) {
                    callback(data.result);
                }
            });
        }

        function update() {
            getTorrents(function(torrents) {
                for (var i = 0; i < torrents.length; i++) {
                    var torrent = torrents[i];
                    var idx = getIndex(torrent.InfoHash);

                    if(idx >= 0) {
                        updateTorrent($scope.torrents[idx], torrent);
                    }
                }

                timer = $timeout(update, 1000);
            });
        }

        var timer = $timeout(function() {
            getTorrents(function(t) {
                $scope.torrents = t;
                timer = $timeout(update, 1000);
            });
        }, 0);        

        var listener = new eventListener();
        listener.onopen = function() {
            listener.subscribe("torrent.added", function(t) { $scope.torrents.push(t); });
            listener.subscribe("torrent.removed", function(infoHash) {
                var idx = getIndex(infoHash);
                $scope.torrents.splice(idx, 1);
            });
        };

        $scope.$on('$destroy', function() {
            $timeout.cancel(timer);
            listener.dispose();
        });
    }
]);