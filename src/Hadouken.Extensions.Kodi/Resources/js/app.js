'use strict';

(function(angular, pluginModules) {
    var extensionId = 'notifier.kodi';

    angular.module(extensionId, [
            'ui.router',
            'hadouken.messaging',
            'notifiers.kodi.controllers.settings'
        ])
        .run([
            'messageService', function(messageService) {
                messageService.subscribe('ui.settings.onloaded', function() {
                    messageService.publish('ui.settings.dialogs.add', {
                        extensionId: extensionId,
                        controller: 'Kodi.SettingsController',
                        templateUrl: 'api/extensions/notifier.kodi/views/settings.html'
                    });
                });
            }
        ]);

    pluginModules.push(extensionId);
})(window.angular, window.pluginModules);