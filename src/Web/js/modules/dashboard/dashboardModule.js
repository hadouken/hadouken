angular.module('hadouken.dashboard', [
    'ui.router',
    'hadouken.dashboard.controllers.index',
    'hadouken.dashboard.directives.widget',
    'hadouken.dashboard.directives.widgetContainer',
    'hadouken.dashboard.widgets.pluginList'
])
.config(['$stateProvider', function ($stateProvider) {
    $stateProvider
        .state('ui.dashboard', {
            controller: 'Dashboard.IndexController',
            url: '/dashboard',
            templateUrl: 'views/dashboard/index.html'
        });
}]);