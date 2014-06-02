angular.module('hadouken.plugins.controllers.list', [
    'ui.bootstrap',
    'hadouken.plugins.controllers.details'
])
.controller('Plugins.ListController', [
    '$scope', '$modal', 'jsonrpc',
    function ($scope, $modal, jsonrpc) {
        jsonrpc.request('core.plugins.list', {
            success: function(data) {
                $scope.plugins = data.result;
            }
        });

        $scope.details = function(pluginId) {
            var detailsModal = $modal.open({
                controller: 'Plugins.DetailsController',
                templateUrl: 'views/plugins/details.html',
                resolve: {
                    pluginId: function() {
                        return pluginId;
                    }
                }
            });
            
            detailsModal.result.then(function (result) {
                if (!result) {
                    return;
                }

                for (var i = 0; i < $scope.plugins.length; i++) {
                    if ($scope.plugins[i].Id === pluginId) {
                        $scope.plugins.splice(i, 1);
                        break;
                    }
                }
            });
        }
    }
]);