angular.module('hadouken.dashboard.directives.widgetContainer', [
])
.directive('widgetContainer', function($document, $compile, $http) {
    return {
        link: function(scope, element, attrs) {
            scope.$watchCollection(attrs.widgets, function(newValue) {
                var widget = newValue[0];

                if(typeof widget === 'undefined') {
                    return;
                }

                var html = '<ng-include src="\'' + widget.templateUrl + '\'" />';
                var el = $compile(html)(scope);

                element.find('.widget-row').append(el);
            });
        },
        restrict: 'E',
        scope: {
            widgets: '='
        },
        templateUrl: 'views/dashboard/_widgetContainer.html'
    }
});