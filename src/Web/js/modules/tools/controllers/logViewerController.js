angular.module('hadouken.tools.controllers.logViewer', [])
    .controller('Tools.LogViewerController', [
        '$scope', 'jsonrpc',
        function ($scope, jsonrpc) {
            jsonrpc.request('core.logging.getEntries', {
                success: function(data) {
                    $scope.entries = data.result;
                }
            });
        }
    ]);