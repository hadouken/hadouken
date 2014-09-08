angular.module('hadouken.settings', [
    'ui.router',
    'hadouken.settings.controllers.index'
])
.config(['$stateProvider', function ($stateProvider) {
    $stateProvider
        .state('ui.settings', {
            controller: 'Settings.IndexController',
            url: '/settings',
            templateUrl: 'views/settings/index.html'
        });
}]);