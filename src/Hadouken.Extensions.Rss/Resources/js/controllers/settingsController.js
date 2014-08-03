angular.module('plugins.rss.controllers.settings', [
    'ui.bootstrap',
    'hadouken.jsonrpc',
    'plugins.rss.controllers.addFeed',
    'plugins.rss.controllers.upsertFilter'
])
.controller('Rss.SettingsController', [
    '$scope', '$modal', 'jsonrpc',
    function ($scope, $modal, jsonrpc) {
        $scope.feeds = [];

        $scope.addFeed = function () {
            var addDialog = $modal.open({
                controller: 'Rss.AddFeedController',
                templateUrl: 'api/extensions/plugin.rss/views/add-feed.html'
            });

            addDialog.result.then(function (feed) {
                console.log(feed);
                $scope.feeds.push(feed);
            });
        };

        $scope.getFilters = function (feedId) {
            $scope.filters = [];

            jsonrpc.request('rss.filters.getByFeedId', {
                params: [feedId],
                success: function(data) {
                    $scope.filters = data.result;
                }
            });
        };

        $scope.upsertFilter = function(feed, filter) {
            var upsertDialog = $modal.open({
                controller: 'Rss.UpsertFilterController',
                templateUrl: 'api/extensions/plugin.rss/views/upsert-filter.html',
                resolve: {
                    feed: function () { return feed; },
                    filter: function() { return filter || { Id: -1, FeedId: feed.Id }; }
                }
            });

            upsertDialog.result.then(function(f) {
                if (!filter) {
                    $scope.filters.push(f);
                }
            });
        };

        jsonrpc.request('rss.feeds.getAll', {
            success: function (data) {
                $scope.feeds = data.result;
            }
        });
    }
]);