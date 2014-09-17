angular.module('notifiers.mailer.controllers.settings', [
    'hadouken.jsonrpc',
    'ui.bootstrap'
])
.controller('Mailer.SettingsController', [
    '$scope', '$modalInstance', 'jsonrpc',
    function ($scope, $modalInstance, jsonrpc) {
        jsonrpc.request('mailer.config.get', {
            success: function (data) {
                $scope.config = data.result;
            }
        });

        $scope.save = function(config) {
            jsonrpc.request('mailer.config.set', {
                params: [config],
                success: function() {
                    $modalInstance.dismiss('close');
                }
            });
        };
    }
]);