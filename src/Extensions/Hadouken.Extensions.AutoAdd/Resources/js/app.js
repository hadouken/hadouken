"use strict";

(function(angular, pluginModules) {
    var extensionId = "plugin.autoadd";

    angular.module(extensionId, [
            "ui.router",
            "hadouken.messaging",
            "plugins.autoadd.controllers.settings"
        ])
        .config([
            "$stateProvider", function($stateProvider) {
                $stateProvider
                    .state("ui.autoaddPluginSettings", {
                        controller: "AutoAdd.SettingsController",
                        url: "/extensions/autoadd/settings",
                        templateUrl: "api/extensions/plugin.autoadd/views/settings.html"
                    });
            }
        ])
        .run([
            "messageService", function(messageService) {
                messageService.subscribe("ui.onloaded", function() {
                    messageService.publish("ui.settings.menuItem.add", {
                        label: "AutoAdd",
                        state: "ui.autoaddPluginSettings"
                    });
                });
            }
        ]);

    pluginModules.push(extensionId);
})(window.angular, window.pluginModules);