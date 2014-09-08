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
                $scope.feeds.push(feed);
            });
        };

        $scope.deleteFeed = function(feed, index) {
            jsonrpc.request('rss.feeds.delete', {
                params: [feed.Id],
                success: function() {
                    $scope.feeds.splice(index, 1);
                }
            });
        };

        $scope.updateFeed = function(feed) {
            jsonrpc.request('rss.feeds.update', {
                params: [feed],
                success: function() {}
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

        $scope.removeFilter = function(filter, index) {
            jsonrpc.request('rss.filters.delete', {
                params: [filter.Id],
                success: function() {
                    $scope.filters.splice(index, 1);
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