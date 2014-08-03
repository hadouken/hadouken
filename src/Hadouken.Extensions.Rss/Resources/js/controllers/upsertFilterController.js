angular.module('plugins.rss.controllers.upsertFilter', [
    'hadouken.jsonrpc'
])
.controller('Rss.UpsertFilterController', [
    '$scope', '$modalInstance', 'jsonrpc', 'feed', 'filter',
    function ($scope, $modalInstance, jsonrpc, feed, filter) {
        $scope.feed = feed;
        $scope.filter = filter;
        $scope.modifiers = [];

        $scope.validate = function () {
            if (typeof filter.IncludePattern === 'undefined' || filter.IncludePattern === '') return false;
            return true;
        };

        $scope.addModifier = function () {
            $scope.modifiers.push({
                Target: "SavePath"
            });
        };

        $scope.removeModifier = function (index) {
            // remove modifier

            $scope.modifiers.splice(index, 1);
        };

        $scope.save = function () {
            if ($scope.filter.Id < 0) {
                jsonrpc.request('rss.filters.create', {
                    params: [$scope.filter],
                    success: function(filterData) {
                        $modalInstance.close(filterData.result);
                    }
                });
            }
        };
    }
]);