angular.module('hadouken.dashboard.directives.widgetContainer', [
])
.directive('widgetContainer', function($compile, $http) {
    var sizes = {
        small: 4,
        medium: 8,
        large: 12
    };

    function addWidget(scope, element, widget) {
        widget.element.appendTo(element.find('.widget-row'));
        widget.active = true;
    };

    function removeWidget(scope, element, widget) {
        widget.element.remove();
        widget.scope.$destroy();
        widget.active = false;
    };

    return {
        controller: function($scope) {
            $scope.registeredWidgets = {};

            $scope.toggle = function(widgetId) {
                var widget = $scope.registeredWidgets[widgetId];
                if(widget.active) {
                    widget.destroy();
                } else {
                    widget.create();
                }
            }
        },
        link: function(scope, element, attrs) {
            scope.$watchCollection(attrs.widgets, function(newValue) {
                var widget = newValue[0];
                if(!widget) { return; }

                var widgetScope = scope.$new();
                widgetScope.widgetId = widget.id;
                widgetScope.title = widget.title;
                widgetScope.hasConfiguration = widget.hasConfiguration;
                widgetScope.size = widget.size;

                (function(scope, wscope, data) {
                    wscope.addWidget = function(id, s, e) {
                        var widget = scope.registeredWidgets[id];
                        widget.element = e;
                        widget.scope = s;

                        addWidget(scope, element, widget);
                    };

                    wscope.removeWidget = function(id) {
                        var widget = scope.registeredWidgets[id];
                        removeWidget(scope, element, widget);
                    };

                    scope.registeredWidgets[data.id] = {
                        title: data.title,
                        create: function() {
                            $http.get(data.templateUrl)
                                .success(function(html) {
                                    $compile(html)(wscope);
                                });
                        },
                        destroy: function() {
                            var widget = scope.registeredWidgets[data.id];
                            removeWidget(scope, element, widget);
                        }
                    };
                })(scope, widgetScope, widget);
            });
        },
        restrict: 'E',
        scope: {
            widgets: '='
        },
        templateUrl: 'views/dashboard/_widgetContainer.html'
    }
});