angular.module('hadouken.dashboard.widgets.pluginList', [
    'hadouken.jsonrpc',
    'hadouken.messaging'
])
.controller('Widgets.PluginListController', function($scope, jsonrpc) {
    $scope.display = 'normal';

    jsonrpc.request('core.plugins.list', {
        success: function(data) {
            $scope.plugins = data.result;
        }
    });

    $scope.showConfiguration = function() {
        $scope.display = 'config';
    };
})
.run(['messageService', 'jsonrpc', function(messageService, jsonrpc) {
    messageService.subscribe('hadouken.dashboard.onloaded', function() {
        var widget = { templateUrl: 'views/dashboard/widgets/pluginList.html' };
        messageService.publish('hadouken.dashboard.widgets.add', widget);
    });
}]);
