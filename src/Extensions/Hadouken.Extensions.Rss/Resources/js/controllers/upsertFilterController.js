angular.module("plugins.rss.controllers.upsertFilter", [
        "hadouken.jsonrpc"
    ])
    .controller("Rss.UpsertFilterController", [
        "$scope", "$modalInstance", "jsonrpc", "feed", "filter",
        function($scope, $modalInstance, jsonrpc, feed, filter) {
            $scope.feed = feed;
            $scope.filter = filter;
            $scope.modifiers = [];

            function asyncLoop(iterations, func, callback) {
                var index = 0;
                var done = false;
                var loop = {
                    next: function() {
                        if (done) {
                            return;
                        }

                        if (index < iterations) {
                            index++;
                            func(loop);

                        } else {
                            done = true;
                            callback();
                        }
                    },

                    iteration: function() {
                        return index - 1;
                    },

                    break: function() {
                        done = true;
                        callback();
                    }
                };
                loop.next();
                return loop;
            };

            // Save modifiers
            function saveModifiers(callback) {
                if ($scope.modifiers.length === 0) {
                    callback();
                    return;
                }

                var length = $scope.modifiers.length;

                asyncLoop(length, function(loop) {
                    var modifier = $scope.modifiers[loop.iteration()];
                    modifier.FilterId = $scope.filter.Id;

                    var method = modifier.Id <= 0 ? "rss.modifiers.create" : "rss.modifiers.update";

                    jsonrpc.request(method, {
                        params: [modifier],
                        success: function() { loop.next(); }
                    });

                }, function() { callback(); });
            };

            $scope.validate = function() {
                if (typeof filter.IncludePattern === "undefined" || filter.IncludePattern === "") return false;
                return true;
            };

            $scope.addModifier = function() {
                $scope.modifiers.push({
                    Id: -1,
                    Target: "SavePath"
                });
            };

            $scope.removeModifier = function(modifier, index) {
                if (modifier.Id <= 0) {
                    $scope.modifiers.splice(index, 1);
                } else {
                    jsonrpc.request("rss.modifiers.delete", {
                        params: [modifier.Id],
                        success: function() {
                            $scope.modifiers.splice(index, 1);
                        }
                    });
                }
            };

            $scope.save = function() {
                if ($scope.filter.Id <= 0) {
                    jsonrpc.request("rss.filters.create", {
                        params: [$scope.filter],
                        success: function(filterData) {
                            $scope.filter = filterData.result;

                            saveModifiers(function() {
                                $modalInstance.close(filterData.result);
                            });
                        }
                    });
                } else {
                    jsonrpc.request("rss.filters.update", {
                        params: [$scope.filter],
                        success: function() {
                            saveModifiers(function() {
                                $modalInstance.close($scope.filter);
                            });
                        }
                    });
                }
            };

            // Get modifiers for this filter
            jsonrpc.request("rss.modifiers.getByFilterId", {
                params: [filter.Id],
                success: function(modifiersData) {
                    $scope.modifiers = modifiersData.result;
                }
            });
        }
    ]);