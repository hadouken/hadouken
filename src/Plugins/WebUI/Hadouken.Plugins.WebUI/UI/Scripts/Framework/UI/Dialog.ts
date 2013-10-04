module Hadouken.UI {
    export class Dialog {
        private _content: any;

        constructor(public url: string) { }

        show(): void {
            $.get(this.url, (html) => {
                console.log('get get get');

                this._content = $(html);
                this._content.modal();

                this.onShow();
            });
        }

        getContent(): any {
            return this._content;
        }

        onShow(): void {
        }
    }
}