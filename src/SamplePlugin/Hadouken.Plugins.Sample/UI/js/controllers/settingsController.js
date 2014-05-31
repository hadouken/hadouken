angular.module('sample.controllers.settings', [
])
.controller('Sample.SettingsController', [
    '$scope', function($scope) {
        $scope.title = 'This is the settings for our Sample plugin';
    }
]);