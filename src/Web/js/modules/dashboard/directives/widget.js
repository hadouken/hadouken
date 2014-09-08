angular.module('hadouken.dashboard.directives.widget', [
])
.directive('widget', function($window) {
    var classes = {
        small: 4,
        medium: 8,
        large: 12
    };

    return {
        restrict: 'E',
        templateUrl: 'views/dashboard/_widget.html',
        transclude: true,
        scope: {
        },
        controller: '@',
        name: 'controller',

        link: function(scope, element, attr, controller, transclude) {
            var parent = scope.$parent;
            var id = parent.widgetId;

            // Copy items from parent
            scope.title = parent.title;
            scope.hasConfiguration = parent.hasConfiguration;
            scope.size = parent.size;

            scope.display = 'normal';
            scope.saveConfig = function() {
                $window.localStorage.setItem('widget_' + id, angular.toJson(scope.config));
                scope.display = 'normal';
            };

            scope.remove = function() {
                parent.removeWidget(id);
            };

            scope.config = angular.fromJson($window.localStorage.getItem('widget_' + id));
            scope.size = scope.size || 'small';
            scope.sizeClass = 'col-sm-' + classes[scope.size];

            transclude(scope, function(clone, scope) {
                element.find('.widget-body').append(clone);
                // Tell our container that we are created.
                parent.addWidget(id, scope, element);
            });
        }
    }
});