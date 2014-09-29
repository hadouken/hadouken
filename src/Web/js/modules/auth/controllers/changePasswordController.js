angular.module('hadouken.auth.controllers.changePassword', [
    'hadouken.auth.services.authProvider',
    'hadouken.jsonrpc',
    'ui.bootstrap'
])
.controller('Auth.ChangePasswordController', [
    '$scope', '$modalInstance', 'authProvider', 'jsonrpc',
    function ($scope, $modalInstance, authProvider, jsonrpc) {
        $scope.userName = authProvider.getUserName();

        $scope.valid = function(currentPassword, newPassword, repeatPassword) {
            if(typeof currentPassword === 'undefined' || currentPassword === '') return false;
            if(typeof newPassword === 'undefined' || newPassword === '') return false;
            if(typeof repeatPassword === 'undefined' || repeatPassword === '') return false;

            if(newPassword !== repeatPassword) return false;

            return true;
        }

        $scope.changePassword = function(userName, currentPassword, newPassword) {
            $scope.busy = true;

            jsonrpc.request('users.changePassword', {
                params: [userName,currentPassword,newPassword],
                success: function() {
                    $modalInstance.close(true);
                }
            });
        }
    }
]);