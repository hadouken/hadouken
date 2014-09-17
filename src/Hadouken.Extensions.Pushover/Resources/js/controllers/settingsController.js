angular.module('notifiers.pushover.controllers.settings', [
    'hadouken.jsonrpc',
    'ui.bootstrap'
])
.controller('Pushover.SettingsController', [
    '$scope', '$modalInstance', 'jsonrpc',
    function ($scope, $modalInstance, jsonrpc) {
        jsonrpc.request('pushover.config.get', {
            success: function (data) {
                $scope.config = data.result;
            }
        });

        $scope.save = function(config) {
            jsonrpc.request('pushover.config.set', {
                params: [config],
                success: function() {
                    $modalInstance.dismiss('close');
                }
            });
        };
    }
]);