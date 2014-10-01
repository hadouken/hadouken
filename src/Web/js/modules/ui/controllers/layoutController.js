angular.module('hadouken.ui.controllers.layout', [
    'hadouken.auth.controllers.apiKeys',
    'hadouken.auth.controllers.changePassword',
    'hadouken.auth.services.authProvider',
    'hadouken.messaging',
    'ui.bootstrap'
])
.controller('UI.LayoutController', [
    '$scope', '$sce', '$window', '$modal', 'authProvider', 'messageService',
    function ($scope, $sce, $window, $modal, authProvider, messageService) {
        $scope.settingsItems = {};
        $scope.menuItems = {};
        $scope.notifications = [];
        $scope.userName = authProvider.getUserName();

        messageService.subscribe('ui.settings.menuItem.add', function(event, params) {
            if ($scope.settingsItems[params.state]) return;
            $scope.settingsItems[params.state] = params;
        });

        messageService.subscribe('ui.menuItem.add', function(event, params) {
            if ($scope.menuItems[params.state]) return;
            $scope.menuItems[params.state] = params;
        });
        messageService.subscribe('ui.notifications.show', function(event, params) {
            params.message = $sce.trustAsHtml(params.message);
            $scope.notifications.push(params);
        });

        messageService.publish('ui.onloaded', {});

        $scope.removeNotification = function(index) {
            var notif = $scope.notifications[index];

            if(typeof notif.onClose === 'function') {
                notif.onClose();
            }

            $scope.notifications.splice(index, 1);
        };

        $scope.showChangePassword = function() {
            $modal.open({
                controller: 'Auth.ChangePasswordController',
                templateUrl: 'views/auth/change-password.html'
            });
        };

        $scope.showApiKeys = function() {
            $modal.open({
                controller: 'Auth.ApiKeysController',
                templateUrl: 'views/auth/api-keys.html'
            });
        };

        $scope.logout = function() {
            authProvider.clearToken();
            $window.location.reload();
        };
    }
]);