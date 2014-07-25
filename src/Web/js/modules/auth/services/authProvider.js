angular.module('hadouken.auth.services.authProvider', [
])
.factory('authProvider', ['$window', function($window) {
    return {
        setToken: function(token) {
            $window.sessionStorage.token = token;
        },

        clearToken: function() {
            $window.sessionStorage.removeItem('token');
        },

        getToken: function() {
            return $window.sessionStorage.token;
        },

        isAuthenticated: function () {
            if (typeof $window.sessionStorage.token === 'undefined') {
                return false;
            }

            return $window.sessionStorage.token != null;
        }
    }
}]);