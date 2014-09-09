'use strict';

(function(angular, pluginModules) {
    var extensionId = 'notifier.pushalot';

    angular.module(extensionId, [
            'ui.router',
            'hadouken.messaging',
            'notifiers.pushalot.controllers.settings'
        ])
        .run([
            'messageService', function(messageService) {
                messageService.subscribe('ui.settings.onloaded', function() {
                    messageService.publish('ui.settings.dialogs.add', {
                        extensionId: extensionId,
                        controller: 'Pushalot.SettingsController',
                        templateUrl: 'api/extensions/notifier.pushalot/views/settings.html'
                    });
                });
            }
        ]);

    pluginModules.push(extensionId);
})(window.angular, window.pluginModules);