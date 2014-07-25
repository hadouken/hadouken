angular.module('hadouken.jsonrpc', [
])
.factory('jsonrpc', ['$http', function ($http) {
    var requestId = 1;

    return {
        request: function (method, options) {
            options.error = options.error || function () { };

            $http.post('/api/jsonrpc', {
                id: requestId,
                jsonrpc: '2.0',
                method: method,
                params: options.params
            }).success(function (data) {
                if (data.error) {
                    options.error(data);
                } else {   
                    options.success(data);
                }
            }).error(function(data) {
                options.error(data);
            });

            requestId += 1;
        }
    };
}]);