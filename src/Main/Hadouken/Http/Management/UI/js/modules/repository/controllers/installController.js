angular.module('hadouken.repository.controllers.install', [
    'hadouken.jsonrpc'
])
.controller('Repository.InstallController', function($scope, $modalInstance, jsonrpc, pkg) {
    $scope.package = pkg;

    $scope.isObject = function(obj) {
        return typeof obj === 'object';
    };

    $scope.install = function (packageId, version, password) {
        $scope.isInstalling = true;

        jsonrpc.request('core.plugins.install', {
            params: [password, packageId, version, false, true],
            success: function() {
                $modalInstance.close(true);
            },
            error: function(data) {
                if (data.error.code === 999) {
                    $scope.invalidPassword = true;
                }

                $scope.isInstalling = false;
            }
        });
    };
});