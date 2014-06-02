angular.module('hadouken.auth.interceptors.tokenHeaderInterceptor', [
    'hadouken.auth.services.authProvider'
])
.factory('tokenHeaderInterceptor', ['authProvider', function (authProvider) {
    return {
        request: function (config) {
            config.headers = config.headers || {};

            if (authProvider.isAuthenticated()) {
                config.headers.Authorization = 'Token ' + authProvider.getToken();
            }

            return config;
        }
    }
}]);