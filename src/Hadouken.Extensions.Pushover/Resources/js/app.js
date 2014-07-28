'use strict';

angular.module('notifiers.pushover', [
    'ui.router',
    'hadouken.messaging',
    'notifiers.pushover.controllers.settings'
])
.config(['$stateProvider', function ($stateProvider) {
    $stateProvider
        .state('ui.pushoverExtensionSettings', {
            controller: 'Pushover.SettingsController',
            url: '/extensions/notifiers/pushover/settings',
            templateUrl: 'api/extensions/notifier.pushover/views/settings.html'
        });
}])
.run(['messageService', function (messageService) {
    messageService.subscribe('ui.onloaded', function () {
        messageService.publish('ui.settings.menuItem.add', {
            label: 'Pushover',
            state: 'ui.pushoverExtensionSettings'
        });
    });
}]);

pluginModules.push('notifiers.pushover');