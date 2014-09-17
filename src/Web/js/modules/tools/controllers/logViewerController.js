angular.module('hadouken.tools.controllers.logViewer', [
    'ui.bootstrap'
])
.filter('truncate', function() {
    return function(source) {
        return source.split('.').pop();
    }
})
.controller('Tools.LogViewerController',
    function ($scope, $modal, jsonrpc) {
        $scope.getLabel = function(level) {
            switch(level) {
                case 'Error':
                    return 'label-danger';
                case 'Info':
                    return 'label-info';
            }

            return 'label-default';
        };

        $scope.showException = function(exception) {
            $modal.open({
                template: '<div class="modal-header"><h3 class="modal-title">View exception</h3></div><div class="modal-body"><pre>' + exception + '</pre></div>'
            });
        };

        jsonrpc.request('logging.getEntries', {
            success: function(data) {
                $scope.entries = data.result.reverse();
            }
        });
    }
);