'use strict';

var extensionId = 'notifier.pushover';

angular.module(extensionId, [
    'ui.router',
    'hadouken.messaging',
    'notifiers.pushover.controllers.settings'
])
.run(['messageService', function (messageService) {
    messageService.subscribe('ui.settings.onloaded', function () {
        messageService.publish('ui.settings.dialogs.add', {
            extensionId: extensionId,
            controller: 'Pushover.SettingsController',
            templateUrl: 'api/extensions/notifier.pushover/views/settings.html'
        });
    });
}]);

pluginModules.push(extensionId);