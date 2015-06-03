angular.module("plugins.automove.controllers.settings", [
        "ui.bootstrap",
        "hadouken.jsonrpc",
        "plugins.automove.controllers.upsertRule"
    ])
    .controller("AutoMove.SettingsController", [
        "$scope", "$modal", "jsonrpc",
        function($scope, $modal, jsonrpc) {
            $scope.rules = [];

            $scope.upsertRule = function(rule) {
                var upsertDialog = $modal.open({
                    controller: "AutoMove.UpsertRuleController",
                    templateUrl: "api/extensions/plugin.automove/views/upsert-rule.html",
                    resolve: {
                        rule: function() { return rule || { Id: -1 }; }
                    }
                });

                upsertDialog.result.then(function(r) {
                    if (!rule) {
                        $scope.rules.push(r);
                    }
                });
            };

            $scope.removeRule = function(rule, index) {
                jsonrpc.request("automove.rules.delete", {
                    params: [rule.Id],
                    success: function() {
                        $scope.rules.splice(index, 1);
                    }
                });
            };

            jsonrpc.request("automove.rules.getAll", {
                success: function(data) {
                    $scope.rules = data.result;
                }
            });
        }
    ]);