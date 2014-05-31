angular.module('hadouken.auth', [
    'ui.router',
    'hadouken.auth.controllers.login',
    'hadouken.auth.interceptors.unauthorizedInterceptor',
    'hadouken.auth.interceptors.tokenHeaderInterceptor',
    'hadouken.auth.services.authProvider'
])
.config(['$httpProvider', '$stateProvider', function ($httpProvider, $stateProvider) {
    $httpProvider.interceptors.push('tokenHeaderInterceptor');

    $stateProvider
        .state('login', {
            controller: 'Auth.LoginController',
            url: '/login?returnState',
            templateUrl: 'views/auth/login.html'
        });
}])
.run(['$rootScope', '$state', '$location', 'authProvider', function($rootScope, $state, $location, authProvider) {
    $rootScope.$on('$stateChangeStart', function (event, toState) {
        $rootScope.latestState = toState;

        var isAuthenticated = authProvider.isAuthenticated();
        
        if (toState.data && toState.data.secure && !isAuthenticated) {
            $location.path('/login').search({ returnState: toState.name });
        }
    });
}]);