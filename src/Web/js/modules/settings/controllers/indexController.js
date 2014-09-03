angular.module('hadouken.settings.controllers.index', [
    'hadouken.messaging',
    'ui.bootstrap'
])
.controller('Settings.IndexController', [
    '$scope', '$http', '$modal', 'messageService', 'jsonrpc',
    function ($scope, $http, $modal, messageService, jsonrpc) {
        var dialogs = {};
        $scope.advancedSettings = {};
        $scope.advancedSettingsBusy = false;

        function getType(obj) {
            if(typeof obj === 'number' && !isNaN(obj) && Math.round(obj) != obj) {
                return 'float';
            }

            return typeof obj;
        };

        jsonrpc.request('config.getMany', {
            params: [''],
            success: function(data) {
                $scope.config = data.result;

                var keys = Object.keys($scope.config);

                for(var i = 0; i < keys.length; i++) {
                    var key = keys[i];

                    if(key.lastIndexOf('bt.', 0) === 0) {
                        $scope.advancedSettings[key] = {
                            type: getType($scope.config[key]),
                            value: $scope.config[key]
                        }
                    }
                }
            }
        });

        jsonrpc.request('extensions.getAll', {
            success: function(data) {
                $scope.extensions = data.result;
            }
        });

        $scope.save = function() {
        };

        $scope.saveAdvanced = function() {
            $scope.advancedSettingsBusy = true;
            var cfg = {};

            for(var key in $scope.advancedSettings) {
                if($scope.advancedSettings[key].dirty) {
                    cfg[key] = $scope.advancedSettings[key].value;
                    $scope.advancedSettings[key].dirty = false;
                }
            }

            jsonrpc.request('config.setMany', {
                params: [cfg],
                success: function() {
                    $scope.advancedSettingsBusy = false;
                }
            });
        };

        $scope.configure = function(extensionId) {
            var dialog = dialogs[extensionId];

            if(!dialog) {
                throw new Error('Invalid extensionId: ' + extensionId);
            }

            $modal.open({
                controller: dialog.controller,
                templateUrl: dialog.templateUrl,
                size: dialog.size || 'md'
            });
        };

        $scope.getType = function(value) {
            return typeof value;
        }

        $scope.toggleExtension = function(extensionId, enabled) {
            var method = enabled ? 'extensions.enable' : 'extensions.disable';
            jsonrpc.request(method, {
                params: [extensionId],
                success: function() {}
            });
        };

        $scope.hasDialog = function(extensionId) {
            return typeof dialogs[extensionId] !== 'undefined';
        };

        var subscription = messageService.subscribe('ui.settings.dialogs.add', function(event, params) {
            dialogs[params.extensionId] = params;
        });

        messageService.publish('ui.settings.onloaded', {});

        $scope.$on('$destroy', function() {
            subscription();
        });
    }
]);