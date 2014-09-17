angular.module('notifiers.pushbullet.controllers.settings', [
    'hadouken.jsonrpc',
    'ui.bootstrap'
])
.controller('Pushbullet.SettingsController', [
    '$scope', '$modalInstance', 'jsonrpc',
    function ($scope, $modalInstance, jsonrpc) {
        jsonrpc.request('pushbullet.config.get', {
            success: function (data) {
                $scope.config = data.result;
            }
        });

        $scope.save = function (config) {
            jsonrpc.request('pushbullet.config.set', {
                params: [config],
                success: function () {
                    $modalInstance.dismiss('close');
                }
            });
        };
    }
]);