angular.module('hadouken.auth.interceptors.unauthorizedInterceptor', [
])
.config(['$httpProvider', function($httpProvider) {
    $httpProvider.interceptors.push([
        '$rootScope', '$q', '$injector', function ($rootScope, $q, $injector) {
            return {
                responseError: function (response) {
                    if (response.status === 401) {
                        var $state = $injector.get('$state');

                        var authProvider = $injector.get('authProvider');
                        authProvider.clearToken();

                        // Do not change state if we set the _silent flag
                        if (response.config._silent) return $q.reject(response);

                        $state.go('login', {
                            returnState: $rootScope.latestState.name
                        }, { location: true, inherit: true, relative: $rootScope.latestState, notify: true });
                    }

                    return $q.reject(response);
                }
            }
        }
    ]);
}]);