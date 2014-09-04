angular.module('hadouken.tools.controllers.logViewer', [])
    .controller('Tools.LogViewerController', [
        '$scope', 'jsonrpc',
        function ($scope, jsonrpc) {
            jsonrpc.request('logging.getEntries', {
                success: function(data) {
                    $scope.entries = data.result;
                }
            });
        }
    ]);