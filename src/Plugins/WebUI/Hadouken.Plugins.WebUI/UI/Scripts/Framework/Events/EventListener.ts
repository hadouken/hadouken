///<reference path="Event.ts"/>
///<reference path="EventHandler.ts"/>

module Hadouken.Events {
    export class EventListener {
        private _connection: any = null;
        private _proxy: any = null;
        private _eventHandlers: { [event: string]: EventHandler[]; } = {};

        public connect(): void {
            var host = location.hostname;
            var port = parseInt(location.port, 10) + 1;
            var url = 'http://' + host + ':' + port;

            this._connection = $.hubConnection(url);
            this._proxy = this._connection.createHubProxy('events');

            this._proxy.on('publishEvent', (event: Event) => this.publishEvent(event));

            this._connection.start()
                .done(() => this.publishEvent(new Event("web.signalR.connected", null)))
                .fail(() => this.publishEvent(new Event("web.signalR.fail", null)));
        }

        private publishEvent(event: Event): void {
            var name = event.name;

            if (typeof this._eventHandlers[name] === "undefined")
                return;

            var handlers = this._eventHandlers[name];

            for (var i = 0; i < handlers.length; i++) {
                handlers[i].handle(event.data);
            }
        }

        public addHandler(name: string, callback: { (data: any): void; }): void {

            if (typeof this._eventHandlers[name] === "undefined") {
                this._eventHandlers[name] = new Array<EventHandler>();
            }

            this._eventHandlers[name].push(new EventHandler(callback));
        }

        public sendEvent(name: string, data: any): void {
            this.publishEvent(new Event('web.' + name, data));
        }
    }
}