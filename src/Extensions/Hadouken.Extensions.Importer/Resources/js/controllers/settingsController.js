angular.module('plugins.importer.controllers.settings', [
    'ui.bootstrap',
    'hadouken.filesystem',
    'hadouken.jsonrpc'
])
.controller('Importer.SettingsController', [
    '$scope', '$modal', 'jsonrpc',
    function ($scope, $modal, jsonrpc) {
        $scope.dataPath = "C:\\Users";

        $scope.import = function(importer, dataPath) {
            $scope.busy = true;

            jsonrpc.request('importer.import', {
                params: [importer, dataPath],
                success: function() {
                    $scope.busy = false;
                }
            });
        };

        jsonrpc.request('importer.getAll', {
            success: function(d) {
                $scope.importers = d.result;
            }
        });
    }
]);