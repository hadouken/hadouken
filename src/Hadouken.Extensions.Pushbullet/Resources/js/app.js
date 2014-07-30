'use strict';

(function(angular, pluginModules) {
    var extensionId = 'notifier.pushbullet';

    angular.module(extensionId, [
            'ui.router',
            'hadouken.messaging',
            'notifiers.pushbullet.controllers.settings'
        ])
        .run([
            'messageService', function(messageService) {
                messageService.subscribe('ui.settings.onloaded', function() {
                    messageService.publish('ui.settings.dialogs.add', {
                        extensionId: extensionId,
                        controller: 'Pushbullet.SettingsController',
                        templateUrl: 'api/extensions/notifier.pushbullet/views/settings.html'
                    });
                });
            }
        ]);

    pluginModules.push(extensionId);
})(window.angular, window.pluginModules);