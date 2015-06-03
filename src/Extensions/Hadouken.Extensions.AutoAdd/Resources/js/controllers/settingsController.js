angular.module("plugins.autoadd.controllers.settings", [
        "ui.bootstrap",
        "hadouken.jsonrpc",
        "plugins.autoadd.controllers.upsertFolder"
    ])
    .controller("AutoAdd.SettingsController", [
        "$scope", "$modal", "jsonrpc",
        function($scope, $modal, jsonrpc) {
            $scope.folders = [];

            $scope.upsertFolder = function(folder) {
                var upsertDialog = $modal.open({
                    controller: "AutoAdd.UpsertFolderController",
                    templateUrl: "api/extensions/plugin.autoadd/views/upsert-folder.html",
                    resolve: {
                        folder: function() { return folder || { Id: -1, Pattern: ".*\\.torrent" }; }
                    }
                });

                upsertDialog.result.then(function(f) {
                    if (!folder) {
                        $scope.folders.push(f);
                    }
                });
            };

            $scope.removeFolder = function(folder, index) {
                jsonrpc.request("autoadd.folders.delete", {
                    params: [folder.Id],
                    success: function() {
                        $scope.folders.splice(index, 1);
                    }
                });
            };

            jsonrpc.request("autoadd.folders.getAll", {
                success: function(data) {
                    $scope.folders = data.result;
                }
            });
        }
    ]);