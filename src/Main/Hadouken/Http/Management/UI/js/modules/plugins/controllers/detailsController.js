angular.module('hadouken.plugins.controllers.details', [])
    .controller('Plugins.DetailsController', [
        '$scope', '$modalInstance', 'jsonrpc', 'pluginId',
        function ($scope, $modalInstance, jsonrpc, pluginId) {
            $scope.step = 'details';
            $scope.canUninstall = true;

            jsonrpc.request('core.plugins.getDetails', {
                params: [pluginId],
                success: function (data) {
                    $scope.plugin = data.result;
                }
            });

            $scope.uninstall = function (password) {
                if ($scope.step === 'uninstall') {
                    $scope.uninstalling = true;

                    jsonrpc.request('core.plugins.uninstall', {
                        params: [password, pluginId, $scope.plugin.Version, false, false],
                        success: function() {
                            $modalInstance.close(true);
                        },
                        error: function(data) {
                            if (data.error.code === 9999) {
                                $scope.invalidPassword = true;
                            }
                            $scope.uninstalling = false;
                        }
                    });
                } else {
                    $scope.step = 'uninstall';
                }
            };
        }
    ]);