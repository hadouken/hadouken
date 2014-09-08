'use strict';

(function(angular, pluginModules) {
    var extensionId = 'notifier.mailer';

    angular.module(extensionId, [
            'ui.router',
            'hadouken.messaging',
            'notifiers.mailer.controllers.settings'
        ])
        .run([
            'messageService', function(messageService) {
                messageService.subscribe('ui.settings.onloaded', function() {
                    messageService.publish('ui.settings.dialogs.add', {
                        extensionId: extensionId,
                        controller: 'Mailer.SettingsController',
                        templateUrl: 'api/extensions/notifier.mailer/views/settings.html'
                    });
                });
            }
        ]);

    pluginModules.push(extensionId);
})(window.angular, window.pluginModules);