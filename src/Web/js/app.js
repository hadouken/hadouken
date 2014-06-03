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
    .filter('bytes', function() {
        return function(bytes, precision) {
            if (isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
            if (typeof precision === 'undefined') precision = 1;
            var units = ['bytes', 'kB', 'MB', 'GB', 'TB', 'PB'],
                number = Math.floor(Math.log(bytes) / Math.log(1024));
            return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) +  ' ' + units[number];
        }
    })
    .run(['$rootScope', 'messageService', function ($rootScope, messageService) {
        messageService.publish('hadouken.onloaded', {});

        $rootScope.$on('$stateChangeStart', function (event, data) {
            if (data.controller) {
                $rootScope.controllerName = data.controller.replace('.', '-');
            }
        });
    }]);
})(window);
