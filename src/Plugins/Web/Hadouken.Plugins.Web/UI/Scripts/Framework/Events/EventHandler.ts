module Hadouken.Events {
    export class EventHandler {
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
}