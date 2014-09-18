angular.module('notifiers.kodi.controllers.settings', [
    'hadouken.jsonrpc',
    'ui.bootstrap'
])
.controller('Kodi.SettingsController', [
    '$scope', '$modalInstance', 'jsonrpc',
    function ($scope, $modalInstance, jsonrpc) {
        jsonrpc.request('kodi.config.get', {
            success: function (data) {
                $scope.config = data.result;
            }
        });

        $scope.save = function (config) {
            jsonrpc.request('kodi.config.set', {
                params: [config],
                success: function () {
                    $modalInstance.dismiss('close');
                }
            });
        };
    }
]);