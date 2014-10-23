'use strict';

(function(angular, pluginModules) {
    var extensionId = 'plugin.automove';

    angular.module(extensionId, [
        'ui.router',
        'hadouken.messaging',
        'plugins.automove.controllers.settings'
    ])
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider
            .state('ui.automovePluginSettings', {
                controller: 'AutoMove.SettingsController',
                url: '/extensions/automove/settings',
                templateUrl: 'api/extensions/plugin.automove/views/settings.html'
            });
    }])
    .run(['messageService', function(messageService) {
        messageService.subscribe('ui.onloaded', function () {
            messageService.publish('ui.settings.menuItem.add', {
                label: 'AutoMove',
                state: 'ui.automovePluginSettings'
            });
        });
    }]);

    pluginModules.push(extensionId);
})(window.angular, window.pluginModules);