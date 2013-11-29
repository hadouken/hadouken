define([ 'jquery', 'jquery.signalr' ], function( $ ) {
    function EventListener() {
        this.connection = $.hubConnection('/events');
        this.proxy = null;
        this.handlers = {};
    }

    EventListener.prototype.connect = function() {
        this.proxy = this.connection.createHubProxy('events');
        this.proxy.on('publishEvent', this.publish);

        var that = this;

        this.connection.start().done(function() {
            that.publish({ name: 'web.connected' });
        });
    };

    EventListener.prototype.disconnect = function() {
        this.connection.stop();

        var keys = Object.keys(this.handlers);

        for (var i = 0; i < keys.length; i++) {
            var key = keys[i];
            
            for (var j = 0; j < this.handlers[key].length; j++) {
                delete this.handlers[key][j];
            }

            delete this.handlers[key];
        }
    };

    EventListener.prototype.publish = function(e) {
        if (typeof this.handlers[e.name] === 'undefined') {
            return;
        }
        
        for (var i = 0; i < this.handlers[e.name].length; i++) {
            this.handlers[e.name][i](e.data);
        }
    };

    EventListener.prototype.subscribe = function(name, handler) {
        if (typeof this.handlers[name] === 'undefined') {
            this.handlers[name] = [];
        }

        this.handlers[name].push(handler);
    };

    return EventListener;
});