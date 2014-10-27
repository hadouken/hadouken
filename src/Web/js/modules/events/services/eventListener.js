angular.module('hadouken.events.services.eventListener', [
    'hadouken.auth.services.authProvider'
])
.factory('eventListener', [ '$window', 'authProvider', function($window, authProvider) {
    var host = $window.location.host;
    var token = authProvider.getToken();
    var url = host + "/events?token=" + token;

    if($window.location.protocol === 'https:') {
        url = "wss://" + url;
    } else {
        url = "ws://" + url;
    }

    return function() {
        $ = this;
        $.onclose = function() {};
        $.onopen = function() {};

        $._callbacks = {};

        $._socket = new WebSocket(url);
        $._socket.onclose = function() { $.onclose(); }
        $._socket.onopen = function() { $.onopen(); }

        $._socket.onmessage = function(event) {
            console.log('message received');
            console.log(event);

            var data = JSON.parse(event.data);
            var callbacks = $._callbacks[data.type];

            if(typeof callbacks === "undefined") {
                return;
            }

            for(var i = 0; i < callbacks.length; i++) {
                callbacks[i](data[data.type]);
            }
        }

        $.dispose = function() {
            for(var key in $._callbacks) {
                delete $._callbacks[key];
            }

            $._socket.close();
        }

        $.subscribe = function(id, callback) {
            if(typeof $._callbacks[id] === "undefined") {
                $._callbacks[id] = [];
            }

            $._callbacks[id].push(callback);
        }
    }
}]);