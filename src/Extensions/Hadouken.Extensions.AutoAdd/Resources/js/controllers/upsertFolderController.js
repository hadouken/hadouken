angular.module("plugins.autoadd.controllers.upsertFolder", [
        "hadouken.jsonrpc"
    ])
    .controller("AutoAdd.UpsertFolderController", [
        "$scope", "$modalInstance", "jsonrpc", "folder",
        function($scope, $modalInstance, jsonrpc, folder) {
            $scope.folder = folder;

            $scope.validate = function() {
                if (typeof folder.Path === "undefined" || folder.Path === "") return false;
                return true;
            };

            $scope.save = function() {
                if ($scope.folder.Id <= 0) {
                    jsonrpc.request("autoadd.folders.create", {
                        params: [$scope.folder],
                        success: function(folderData) {
                            $modalInstance.close(folderData.result);
                        }
                    });
                } else {
                    jsonrpc.request("autoadd.folders.update", {
                        params: [$scope.folder],
                        success: function() {
                            $modalInstance.close($scope.filter);
                        }
                    });
                }
            };
        }
    ]);