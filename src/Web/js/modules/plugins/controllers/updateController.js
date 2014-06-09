angular.module('hadouken.plugins.controllers.update', [])
    .controller('Plugins.UpdateController', [
        '$scope', '$modalInstance', 'jsonrpc', 'packageId', 'version',
        function ($scope, $modalInstance, jsonrpc, packageId, version) {
            $scope.packageId = packageId;
            $scope.version = version;

            $scope.update = function(password) {
                $scope.updating = true;
                
                jsonrpc.request('core.plugins.update', {
                    params: [ password, packageId, version, true, false ],
                    success: function() {
                        $modalInstance.close(true);
                    },
                    error: function(data) {
                        if (data.error.code === 9999) {
                            $scope.invalidPassword = true;
                        }
                        $scope.updating = false;
                    }
                });
            };
        }
    ]);