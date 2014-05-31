'use strict';

angular.module('sample', [
    'ui.router',
    'hadouken.messaging',
    'sample.controllers.settings'
])
.config(['$stateProvider', function ($stateProvider) {
    $stateProvider
        .state('ui.samplePluginSettings', {
            controller: 'Sample.SettingsController',
            url: '/plugins/sample/settings',
            templateUrl: 'plugins/sample/views/settings.html'
        });
}])
.run(['messageService', function (messageService) {
    messageService.subscribe('ui.onloaded', function() {
        messageService.publish('ui.settings.menuItem.add', {
            label: 'Sample',
            state: 'ui.samplePluginSettings'
        });
    });
}]);

pluginModules.push('sample');