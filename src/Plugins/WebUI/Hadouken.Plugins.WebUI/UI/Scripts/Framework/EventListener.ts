module Hadouken {
    class EventHandler {
        private _callback: {
            (data: any): void;
        };

        constructor(callback: { (data: any): void; }) {
            this._callback = callback;
        }

        handle(data: any): void {
            this._callback(data);
        }
    }

    class Event {
        constructor(public name: string, public data: any) { }
    }

    export class EventListener {
        private static _instance: EventListener = null;

        private _connection: any = null;
        private _proxy: any = null;
        private _eventHandlers: { [event: string]: EventHandler[]; } = {};

        constructor() {
            if (EventListener._instance) {
                throw new Error("Use EventListener.getInstance();");
            }

            EventListener._instance = this;
        }

        public static getInstance(): EventListener {
            if (EventListener._instance === null) {
                EventListener._instance = new EventListener();
                EventListener._instance.connect();
            }

            return EventListener._instance;
        }

        private connect(): void {
            var host = location.hostname;
            var port = parseInt(location.port, 10) + 1;
            var url = 'http://' + host + ':' + port;

            this._connection = $.hubConnection(url);
            this._proxy = this._connection.createHubProxy('events');

            this._proxy.on('publishEvent', (event: Event) => this.publishEvent(event));

            this._connection.start().done(() => this.publishEvent(new Event("connected", null)));
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
    }
}