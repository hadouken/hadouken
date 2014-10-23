'use strict';

(function(angular, pluginModules) {
    var extensionId = 'plugin.importer';

    angular.module(extensionId, [
        'ui.router',
        'hadouken.messaging',
        'plugins.importer.controllers.settings'
    ])
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider
            .state('ui.importerPluginSettings', {
                controller: 'Importer.SettingsController',
                url: '/extensions/importer/settings',
                templateUrl: 'api/extensions/plugin.importer/views/settings.html'
            });
    }])
    .run(['messageService', function(messageService) {
        messageService.subscribe('ui.onloaded', function () {
            messageService.publish('ui.settings.menuItem.add', {
                label: 'Importer',
                state: 'ui.importerPluginSettings'
            });
        });
    }]);

    pluginModules.push(extensionId);
})(window.angular, window.pluginModules);