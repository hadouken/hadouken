angular.module('hadouken.repository.controllers.list', [
    'ui.bootstrap',
    'hadouken.jsonrpc',
    'hadouken.repository.controllers.install'
])
.controller('Repository.ListController', function($scope, $modal, jsonrpc) {
    $scope.search = function (searchQuery) {
        $scope.packages = [];
        $scope.isSearching = true;

        jsonrpc.request('core.repository.search', {
            params: [1, searchQuery, true],
            success: function (data) {
                $scope.packages = data.result.Items;
                $scope.isSearching = false;
            }
        });
    };

    $scope.prepareInstall = function (packageId, version) {
        $scope.currentInstall = packageId;

        function showModal(pkg) {
            var installModal = $modal.open({
                controller: 'Repository.InstallController',
                templateUrl: 'views/repository/install.html',
                resolve: {
                    pkg: function () { return pkg; }
                }
            });
        };

        jsonrpc.request('core.repository.getDetails', {
            params: [packageId, version],
            success: function(data) {
                showModal(data.result);
            }
        });
        
    };

    $scope.search();
});