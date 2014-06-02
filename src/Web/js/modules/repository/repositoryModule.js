angular.module('hadouken.repository', [
    'hadouken.repository.controllers.list'
])
.config(function($stateProvider) {
    $stateProvider
        .state('ui.repository', {
            controller: 'Repository.ListController',
            url: '/repository',
            templateUrl: 'views/repository/list.html'
        });
})