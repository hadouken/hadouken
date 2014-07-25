angular.module('hadouken.dashboard.controllers.index', [
    'hadouken.messaging'
])
.controller('Dashboard.IndexController', [
    '$scope', 'messageService',
    function($scope, messageService) {
        $scope.widgets = [];

        messageService.subscribe('hadouken.dashboard.widgets.add',
            function(event, widget) {
                $scope.widgets.splice(0, 0, widget);
            });

        messageService.publish('hadouken.dashboard.onloaded', {});
    }
]);