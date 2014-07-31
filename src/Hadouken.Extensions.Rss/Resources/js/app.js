'use strict';

(function(angular, pluginModules) {
    var extensionId = 'plugin.rss';

    angular.module(extensionId, [
            'ui.router',
            'hadouken.messaging',
            'plugins.rss.controllers.settings'
        ])
        .run([
            'messageService', function(messageService) {
                messageService.subscribe('ui.settings.onloaded', function() {
                    messageService.publish('ui.settings.dialogs.add', {
                        extensionId: extensionId,
                        controller: 'Rss.SettingsController',
                        size: 'lg',
                        templateUrl: 'api/extensions/plugin.rss/views/settings.html'
                    });
                });
            }
        ]);

    pluginModules.push(extensionId);
})(window.angular, window.pluginModules);