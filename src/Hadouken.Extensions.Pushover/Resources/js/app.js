'use strict';

angular.module('pushover', [
    'ui.router',
    'hadouken.messaging',
    'pushover.controllers.settings'
])
.config(['$stateProvider', function ($stateProvider) {
    $stateProvider
        .state('ui.pushoverPluginSettings', {
            controller: 'Pushover.SettingsController',
            url: '/plugins/pushover/settings',
            templateUrl: 'api/extensions/notifier.pushover/views/settings.html'
        });
}])
.run(['messageService', function (messageService) {
    messageService.subscribe('ui.onloaded', function () {
        messageService.publish('ui.settings.menuItem.add', {
            label: 'Pushover',
            state: 'ui.pushoverPluginSettings'
        });
    });
}]);

pluginModules.push('pushover');