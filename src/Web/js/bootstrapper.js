'use strict';

window.pluginModules = window.pluginModules || [];

(function ($script) {
    // We need to get all extra background scripts
    // from the API.
    var request = new XMLHttpRequest();
    request.open('GET', '/api/extensions', true);
    request.onreadystatechange = function() {
        if (request.readyState != 4 || request.status != 200) return;
        var additionalScripts = JSON.parse(request.responseText);
        bootstrap(additionalScripts);
    };
    request.setRequestHeader('Accept', 'application/json');
    request.send();

    function bootstrap(additionalScripts) {
        var coreScripts = [
            'js/lib/angular-ui-router.min.js',

            /* Auth module */
            'js/modules/auth/authModule.js',
            'js/modules/auth/controllers/loginController.js',
            'js/modules/auth/interceptors/unauthorizedInterceptor.js',
            'js/modules/auth/interceptors/tokenHeaderInterceptor.js',
            'js/modules/auth/services/authProvider.js',

            /* BitTorrent module */
            'js/modules/bittorrent/bittorrentModule.js',
            'js/modules/bittorrent/controllers/settingsController.js',
            'js/modules/bittorrent/controllers/torrentAddController.js',
            'js/modules/bittorrent/controllers/torrentChangeLabelController.js',
            'js/modules/bittorrent/controllers/torrentDetailsController.js',
            'js/modules/bittorrent/controllers/torrentListController.js',
            'js/modules/bittorrent/controllers/torrentMoveController.js',
            'js/modules/bittorrent/directives/filereadDirective.js',

            /* JSONRPC module */
            'js/modules/jsonrpc/jsonrpcModule.js',

            /* Messaging module */
            'js/modules/messaging/messagingModule.js',

            /* Settings module */
            'js/modules/settings/settingsModule.js',
            'js/modules/settings/controllers/indexController.js',

            /* Tools module */
            'js/modules/tools/toolsModule.js',
            'js/modules/tools/controllers/jsonrpcController.js',
            'js/modules/tools/controllers/logViewerController.js',

            /* UI module */
            'js/modules/ui/uiModule.js',
            'js/modules/ui/controllers/layoutController.js',

            /* Dashboard module */
            'js/modules/dashboard/dashboardModule.js',
            'js/modules/dashboard/controllers/indexController.js',
            'js/modules/dashboard/directives/widget.js',
            'js/modules/dashboard/directives/widgetContainer.js',
            'js/modules/dashboard/widgets/pluginList.js'
        ];

        // Load all core scripts.
        $script(coreScripts, 'core');

        $script.ready('core', function () {
            if (additionalScripts.length) {
                // Core scripts are ready, now load the plugin scripts.
                $script(additionalScripts, function() {

                    // When all core and plugin scripts are ready, load the app script.
                    $script(['js/app.js'], function() {
                        angular.bootstrap(document, ['hadouken']);
                    });
                });
            } else {
                $script(['js/app.js'], function () {
                    angular.bootstrap(document, ['hadouken']);
                });
            }
        });
    }
})(window.$script);