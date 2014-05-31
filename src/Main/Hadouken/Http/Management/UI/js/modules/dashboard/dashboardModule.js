angular.module('hadouken.dashboard', [
    'ui.router',
    'hadouken.dashboard.controllers.index'
])
.config(['$stateProvider', function ($stateProvider) {
    $stateProvider
        .state('ui.dashboard', {
            controller: 'Dashboard.IndexController',
            url: '/dashboard',
            templateUrl: 'views/dashboard/index.html'
        });
}]);