angular.module('hadouken.messaging', [
])
.service('messageService', ['$rootScope', function ($rootScope) {
    return {
        publish: function (name, parameters) {
            $rootScope.$emit(name, parameters);
            var re = new RegExp(/^([^\:]*?)\:.*$/i);
            var res;
            if (!(res = re.exec(name)))
                return;

            var wildCarded = res[1] + ":*";
            $rootScope.$emit(wildCarded, parameters);

        },
        subscribe: function (name, listener) {
            $rootScope.$on(name, listener);
        }
    };
}]);