angular.module('notifiers.pushalot.controllers.settings', [
    'hadouken.jsonrpc',
    'ui.bootstrap'
])
.controller('Pushalot.SettingsController', [
    '$scope', '$modalInstance', 'jsonrpc',
    function ($scope, $modalInstance, jsonrpc) {
        jsonrpc.request('pushalot.config.get', {
            success: function (data) {
                $scope.config = data.result;
            }
        });

        $scope.test = function (config) {
            jsonrpc.request('pushalot.config.test', {
                params: [config],
                success: function () { }
            });
        };

        $scope.save = function (config) {
            jsonrpc.request('pushalot.config.set', {
                params: [config],
                success: function () {
                    $modalInstance.dismiss('close');
                }
            });
        };
    }
]);