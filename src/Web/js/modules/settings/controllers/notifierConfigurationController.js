angular.module('hadouken.settings.controllers.notifierConfiguration', [
    'hadouken.jsonrpc'
])
.controller('Settings.NotifierConfigurationController', [
    '$scope', '$modalInstance', 'jsonrpc', 'config', 'notifier',
    function($scope, $modalInstance, jsonrpc, config, notifier) {
        $scope.config = config;
        $scope.dict = {};
        $scope.notifier = notifier;

        jsonrpc.request('config.get', {
            params: [config.Key],
            success: function(data) {
                if(data.result !== null) {
                    $scope.dict = data.result;
                }
            }
        });

        $scope.save = function() {
            $scope.saving = true;

            jsonrpc.request('config.set', {
                params: [$scope.config.Key, $scope.dict],
                success: function(data) {
                    $modalInstance.dismiss('close');
                }
            });
        }
    }
]);