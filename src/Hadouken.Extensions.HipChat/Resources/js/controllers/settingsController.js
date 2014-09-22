angular.module('notifiers.hipchat.controllers.settings', [
    'hadouken.jsonrpc',
    'ui.bootstrap'
])
.controller('HipChat.SettingsController', [
    '$scope', '$modalInstance', 'jsonrpc',
    function ($scope, $modalInstance, jsonrpc) {
        jsonrpc.request('hipchat.config.get', {
            success: function (data) {
                $scope.config = data.result;

                if (!$scope.config) {
                    $scope.config = { From: 'Hadouken' };
                }
            }
        });

        $scope.save = function (config) {
            jsonrpc.request('hipchat.config.set', {
                params: [config],
                success: function () {
                    $modalInstance.dismiss('close');
                }
            });
        };
    }
]);