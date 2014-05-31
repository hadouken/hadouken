'use strict';

(function (window) {
    var core = [
        'ui.bootstrap',
        'ui.router',
        'hadouken.auth',
        'hadouken.jsonrpc',
        'hadouken.messaging',
        'hadouken.plugins',
        'hadouken.repository',
        'hadouken.settings',
        'hadouken.ui',
        'hadouken.dashboard'
    ];

    angular.module('hadouken', core.concat(window.pluginModules))
    .config(['$urlRouterProvider', function ($urlRouterProvider) {
        $urlRouterProvider.otherwise('/dashboard');
    }])
    .run(['$rootScope', 'messageService', function ($rootScope, messageService) {
        console.log('running core app');

        messageService.publish('hadouken.onloaded', {});

        $rootScope.$on('$stateChangeStart', function (event, data) {
            if (data.controller) {
                $rootScope.controllerName = data.controller.replace('.', '-');
            }
        });
    }]);
})(window);
