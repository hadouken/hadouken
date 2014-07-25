angular.module('hadouken.tools', [
    'ui.router',
    'hadouken.tools.controllers.jsonrpc',
    'hadouken.tools.controllers.logViewer'
])
.config(['$stateProvider', function ($stateProvider) {
    $stateProvider
        .state('ui.tools_jsonrpc', {
            controller: 'Tools.JsonRpcController',
            url: '/tools/jsonrpc',
            templateUrl: 'views/tools/jsonrpc.html'
        })
        .state('ui.tools_logViewer', {
            controller: 'Tools.LogViewerController',
            url: '/tools/log',
            templateUrl: 'views/tools/log.html'
        });
}]);