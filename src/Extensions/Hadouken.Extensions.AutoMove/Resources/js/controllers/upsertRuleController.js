angular.module("plugins.automove.controllers.upsertRule", [
        "hadouken.jsonrpc"
    ])
    .controller("AutoMove.UpsertRuleController", [
        "$scope", "$modalInstance", "jsonrpc", "rule",
        function($scope, $modalInstance, jsonrpc, rule) {
            $scope.rule = rule;
            $scope.parameters = [];

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

            $scope.validate = function() {
                if (rule.Name === "" || rule.TargetPath === "") return false;
                if ($scope.parameters.length === 0) return false;
                return true;
            };

            $scope.addParameter = function() {
                $scope.parameters.push({
                    Id: -1,
                    Source: "Name",
                    Pattern: ".*"
                });
            };

            $scope.removeParameter = function(parameter, index) {
                if (parameter.Id <= 0) {
                    $scope.parameters.splice(index, 1);
                } else {
                    jsonrpc.request("automove.parameters.delete", {
                        params: [parameter.Id],
                        success: function() {
                            $scope.parameters.splice(index, 1);
                        }
                    });
                }
            };

            // Save parameters
            function saveParameters(callback) {
                if ($scope.parameters.length === 0) {
                    callback();
                    return;
                }

                var length = $scope.parameters.length;

                asyncLoop(length, function(loop) {
                    var parameter = $scope.parameters[loop.iteration()];
                    parameter.RuleId = $scope.rule.Id;

                    var method = parameter.Id <= 0 ? "automove.parameters.create" : "automove.parameters.update";

                    jsonrpc.request(method, {
                        params: [parameter],
                        success: function() { loop.next(); }
                    });

                }, function() { callback(); });
            };

            $scope.save = function() {
                if ($scope.rule.Id <= 0) {
                    jsonrpc.request("automove.rules.create", {
                        params: [$scope.rule],
                        success: function(data) {
                            $scope.rule = data.result;

                            saveParameters(function() {
                                $modalInstance.close(data.result);
                            });
                        }
                    });
                } else {
                    jsonrpc.request("automove.rules.update", {
                        params: [$scope.rule],
                        success: function() {
                            saveParameters(function() {
                                $modalInstance.close($scope.rule);
                            });
                        }
                    });
                }
            };

            // Get parameters for this rule
            jsonrpc.request("automove.parameters.getByRuleId", {
                params: [rule.Id],
                success: function(data) {
                    $scope.parameters = data.result;

                    // Add default parameter
                    if ($scope.parameters.length === 0) {
                        $scope.addParameter();
                    }
                }
            });
        }
    ]);