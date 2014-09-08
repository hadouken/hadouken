angular.module('hadouken.ui', [
    'ui.router',
    'hadouken.ui.controllers.layout'
])
.config(['$stateProvider', function($stateProvider) {
    $stateProvider
        .state('ui', {
            controller: 'UI.LayoutController',
            data: {
                secure: true
            },
            templateUrl: 'views/shared/layout.html'
        });
}]);