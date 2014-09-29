angular.module('hadouken.auth.controllers.apiKeys', [
    'hadouken.auth.services.authProvider',
    'hadouken.jsonrpc',
    'ui.bootstrap'
])
.controller('Auth.ApiKeysController', [
    '$scope', '$modalInstance', 'authProvider', 'jsonrpc',
    function ($scope, $modalInstance, authProvider, jsonrpc) {
        $scope.apiKey = authProvider.getToken();

        $scope.close = function() {
            $modalInstance.close(true);
        };
    }
]);