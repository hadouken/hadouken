angular.module('hadouken.settings.controllers.index', [
    'hadouken.messaging',
    'ui.bootstrap',
    'hadouken.settings.controllers.notifierConfiguration'
])
.controller('Settings.IndexController', [
    '$scope', '$http', '$modal', 'messageService', 'jsonrpc',
    function ($scope, $http, $modal, messageService, jsonrpc) {
        var dialogs = {};
        $scope.advancedSettings = {};
        $scope.advancedSettingsBusy = false;
        $scope.busyIndicators = {};

        function getType(obj) {
            if(typeof obj === 'number' && !isNaN(obj) && Math.round(obj) != obj) {
                return 'float';
            }

            return typeof obj;
        };

        function loadNotifications() {
            jsonrpc.request('notifiers.getAllTypes', {
                params: null,
                success: function(data) {
                    $scope.notificationTypes = data.result;

                    jsonrpc.request('notifiers.getAll', {
                        params: null,
                        success: function(data) {
                            $scope.availableNotifiers = data.result;
                            
                            if($scope.selectedNotificationType) {
                                getNotifiersForType($scope.selectedNotificationType);
                            }
                        }
                    });
                }
            });
        };

        function getNotifiersForType(type) {
            $scope.selectedNotificationType = type;
            $scope.notifiers = [];

            for(var i = 0; i < $scope.availableNotifiers.length; i++) {
                var notifier = angular.copy($scope.availableNotifiers[i]);
                notifier.Enabled = notifier.RegisteredTypes.indexOf(type) > -1;

                $scope.notifiers.push(notifier);
            }
        };

        $scope.getNotifiersForType = function(type) {
            getNotifiersForType(type);
        };

        $scope.enableDisableNotifier = function(notifierId, value, type) {
            var method = value ? 'notifiers.register' : 'notifiers.unregister';

            jsonrpc.request(method, {
                params: [notifierId, type],
                success: function() {}
            });
        };

        $scope.testNotifier = function(notifierId) {
            $scope.busyIndicators[notifierId] = 1;

            jsonrpc.request('notifiers.test', {
                params: [notifierId],
                success: function() {
                    delete $scope.busyIndicators[notifierId];
                }
            });
        };

        $scope.save = function() {
            var keys = [
                'bt.save_path',
                'bt.net.listen_port',
                'http.binding',
                'http.port'
            ];

            var data = {};

            for(var i = 0; i < keys.length; i++) {
                data[keys[i]] = $scope.config[keys[i]];
            }

            jsonrpc.request('config.setMany', {
                params: [data],
                success: function() {}
            });
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

        $scope.configure = function(notifier) {
            jsonrpc.request('notifiers.getConfiguration', {
                params: [notifier.Id],
                success: function(data) {
                    if(data.result) {
                        var m = $modal.open({
                            controller: 'Settings.NotifierConfigurationController',
                            templateUrl: 'views/settings/notifier-configuration.html',
                            resolve: {
                                config: function() {
                                    return data.result;
                                },
                                notifier: function() {
                                    return notifier;
                                }
                            }
                        });

                        // Reload all notifications after we have configured one
                        m.result.then(function() { loadNotifications(); }, function() { loadNotifications(); });
                    } else {
                        var dialog = dialogs[notifier.Id];

                        if(!dialog) {
                            throw new Error('Invalid extensionId: ' + extensionId);
                        }

                        var m = $modal.open({
                            controller: dialog.controller,
                            templateUrl: dialog.templateUrl,
                            size: dialog.size || 'md'
                        });

                        // Reload all notifications after we have configured one
                        m.result.then(function() { loadNotifications(); }, function() { loadNotifications(); });
                    }
                }
            });
        };

        $scope.getType = function(value) {
            return typeof value;
        }

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

                loadNotifications();
            }
        });
    }
]);