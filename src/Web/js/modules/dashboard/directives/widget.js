angular.module('hadouken.dashboard.directives.widget', [
])
.directive('widget', function($window) {
    var classes = {
        small: 4,
        medium: 8,
        large: 12
    };

    return {
        link: function(scope, element, attr, controller, transclude) {
            scope.display = 'normal';
            scope.saveConfig = function() {
                $window.localStorage.setItem('widget_' + scope.id, angular.toJson(scope.config));
                scope.display = 'normal';
            };

            scope.config = angular.fromJson($window.localStorage.getItem('widget_' + scope.id));

            transclude(scope, function(clone, scope) {
                scope.size = scope.size || 'small';
                scope.sizeClass = 'col-sm-' + classes[scope.size];
                
                element.find('.widget-body').append(clone);
            });
        },
        restrict: 'E',
        templateUrl: 'views/dashboard/_widget.html',
        transclude: true,
        scope: {
            id: '@',
            hasConfiguration: '&',
            size: '@',
            title: '@'
        },
        controller: '@',
        name: 'controller'
    }
});