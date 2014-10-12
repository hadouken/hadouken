angular.module('plugins.rss.controllers.addFeed', [
    'hadouken.jsonrpc'
])
.controller('Rss.AddFeedController', [
    '$scope', '$modalInstance', 'jsonrpc',
    function ($scope, $modalInstance, jsonrpc) {
        $scope.feed = {
            PollInterval: 15
        };

        $scope.validate = function (feed) {
            if (typeof feed.Name === 'undefined' || feed.Name === '') return false;
            if (typeof feed.Url === 'undefined' || feed.Url === '') return false;

            return true;
        };

        $scope.add = function (feed) {
            jsonrpc.request('rss.feeds.create', {
                params: [feed],
                success: function(f) {
                    $modalInstance.close(f.result);
                }
            });
        };
    }
]);