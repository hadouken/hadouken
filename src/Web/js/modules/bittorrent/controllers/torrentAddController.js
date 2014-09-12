angular.module('hadouken.bittorrent.controllers.torrentAdd', [
    'hadouken.jsonrpc',
    'hadouken.bittorrent.directives.fileread'
])
.controller('BitTorrent.TorrentAddController', [
    '$scope', '$modalInstance', 'jsonrpc',
    function ($scope, $modalInstance, jsonrpc) {
        $scope.source = 'file';

        $scope.add = function (source, fileData, url, name, label, savePath) {
            $scope.inProgress = true;

            var data = null;
            var method = 'torrents.addFile';

            if(source === 'url') {
                data = url;
                method = 'torrents.addUrl';
            } else {
                data = fileData.split(',')[1];
            }

            var addParams = {
                label: label,
                name: name,
                savePath: savePath
            };

            jsonrpc.request(method, {
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