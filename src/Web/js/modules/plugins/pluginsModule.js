angular.module('hadouken.plugins', [
    'hadouken.plugins.controllers.list'
])
.config(['$stateProvider', function ($stateProvider) {
    $stateProvider
        .state('ui.plugins', {
            controller: 'Plugins.ListController',
            url: '/plugins',
            templateUrl: 'views/plugins/list.html'
        });
}]);