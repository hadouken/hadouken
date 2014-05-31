angular.module('hadouken.auth.controllers.login', [
    'ui.router',
    'hadouken.auth.services.authProvider'
])
.controller('Auth.LoginController', [
    '$http', '$scope', '$state', '$stateParams', 'authProvider',
    function ($http, $scope, $state, $stateParams, authProvider) {
        $scope.login = function () {
            $scope.loggingIn = true;

            var user = {
                UserName: $scope.userName,
                Password: $scope.password
            };

            $http.post('/auth/login', user, { _silent: true })
                .success(function(data) {
                    authProvider.setToken(data.token);

                    if ($stateParams.returnState && $stateParams.returnState !== null) {
                        $state.go($stateParams.returnState);
                    } else {
                        $state.go('ui.dashboard');
                    }
                })
                .error(function() {
                    $scope.loggingIn = false;
                });
        }
    }
]);