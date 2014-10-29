angular.module('hadouken.bittorrent.controllers.renameTorrentFile', [
    'hadouken.jsonrpc'
])
.controller('BitTorrent.RenameTorrentFileController', [
    '$scope', '$modalInstance', 'jsonrpc', 'infoHash', 'fileIndex', 'fileName',
    function ($scope, $modalInstance, jsonrpc, infoHash, fileIndex, fileName) {
        $scope.infoHash = infoHash;
        $scope.fileIndex = fileIndex;
        $scope.fileName = fileName;

        $scope.rename = function(infoHash, fileIndex, fileName) {
            $scope.busy = true;
            
            jsonrpc.request('torrents.renameFile', {
                params: [infoHash, fileIndex, fileName],
                success: function() {
                    $modalInstance.dismiss('close');
                }
            });
        };
    }]);