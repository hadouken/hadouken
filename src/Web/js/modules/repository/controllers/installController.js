angular.module('hadouken.repository.controllers.install', [
    'hadouken.jsonrpc'
])
.controller('Repository.InstallController', function($scope, $modalInstance, jsonrpc, pkg) {
    $scope.package = pkg;

    $scope.isPermissionSet = function(obj) {
        return obj.substr(0, 14) === '<PermissionSet';
    };

    if($scope.isPermissionSet(pkg.Permissions)) {
        $scope.permissionsList = getPermissions(pkg.Permissions);
    };

    function getPermissions(xml) {
        var parser = new DOMParser();
        var doc = parser.parseFromString(xml, 'text/xml');
        var root = doc.getElementsByTagName('PermissionSet');

        if(root.length > 0
            && root[0].attributes['Unrestricted']
            && root[0].attributes['Unrestricted'].value === 'true')
        {
            return [{
                name: "Full",
                unrestricted: true
            }];
        }

        var permissionNodes = doc.getElementsByTagName("IPermission");
        var result = [];

        for(var i = 0; i < permissionNodes.length; i++) {
            var node = permissionNodes[i];
            var className = node.attributes['class'].value.split(',');
            var unrestricted = node.attributes['Unrestricted'].value === 'true';
            var lastDot = className[0].lastIndexOf('.');

            result.push({
                name: className[0].substr(lastDot + 1),
                unrestricted: unrestricted
            });
        }

        return result;
    };

    $scope.install = function (packageId, version, password) {
        $scope.isInstalling = true;

        jsonrpc.request('core.plugins.install', {
            params: [password, packageId, version, false, true],
            success: function() {
                $modalInstance.close(true);
            },
            error: function(data) {
                if (data.error.code === 999) {
                    $scope.invalidPassword = true;
                }

                $scope.isInstalling = false;
            }
        });
    };
});