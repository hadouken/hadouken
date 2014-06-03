angular.module('hadouken.dashboard.directives.widget', [
])
.directive('widget', function($document) {
    var classes = {
        small: 4,
        medium: 8,
        large: 12
    };

    return {
        link: function(scope, element, attr, controller, transclude) {
            scope.size = scope.defaultSize || 'small';
            scope.sizeClass = 'col-sm-' + classes[scope.size];

            transclude(scope, function(clone, scope) {
                element.find('.panel-body').append(clone);
            });
        },
        restrict: 'E',
        templateUrl: 'views/dashboard/_widget.html',
        transclude: true,
        scope: {
            defaultSize: '@',
            title: '@'
        },
        controller: '@',
        name: 'controller'
    }
});