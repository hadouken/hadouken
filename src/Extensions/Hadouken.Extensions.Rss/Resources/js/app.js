"use strict";

(function(angular, pluginModules) {
    var extensionId = "plugin.rss";

    angular.module(extensionId, [
            "ui.router",
            "hadouken.messaging",
            "plugins.rss.controllers.settings"
        ])
        .config([
            "$stateProvider", function($stateProvider) {
                $stateProvider
                    .state("ui.rssPluginSettings", {
                        controller: "Rss.SettingsController",
                        url: "/extensions/rss/settings",
                        templateUrl: "api/extensions/plugin.rss/views/settings.html"
                    });
            }
        ])
        .run([
            "messageService", function(messageService) {
                messageService.subscribe("ui.onloaded", function() {
                    messageService.publish("ui.settings.menuItem.add", {
                        label: "RSS",
                        state: "ui.rssPluginSettings"
                    });
                });
            }
        ]);

    pluginModules.push(extensionId);
})(window.angular, window.pluginModules);