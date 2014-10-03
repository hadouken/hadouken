'use strict';

(function (angular, pluginModules) {
    var extensionId = 'plugin.updatechecker';

    angular.module(extensionId, [
        'hadouken.jsonrpc'
    ])
    .run(['$timeout', 'jsonrpc', 'messageService', function ($timeout, jsonrpc, messageService) {
        var sent = false;

        function check(callback) {
            jsonrpc.request('config.getMany', {
                params: ['updatechecker.'],
                success: function(r) {
                    var release = r.result['updatechecker.release'];
                    var muted = r.result['updatechecker.muted'];

                    if (release !== null && typeof release !== 'undefined' && !muted && !sent) {
                        messageService.publish('ui.notifications.show', {
                            type: 'info',
                            message: 'Hadouken <strong>' + release.tag_name + '</strong> released! <a href="' + release.html_url + '" target="_blank">Go to release</a>.',
                            onClose: function() { mute(); }
                        });

                        sent = true;
                    }

                    callback();
                }
            });
        }

        function mute() {
            jsonrpc.request('config.set', {
                params: ['updatechecker.muted', true],
                success: function() {}
            });
        }

        function update() {
            check(function() {
                $timeout(update, 60000); // Check every minute
            });
        }

        messageService.subscribe('ui.onloaded', function() {
            $timeout(update, 0);
        });
    }]);

    pluginModules.push(extensionId);
})(window.angular, window.pluginModules);