module Hadouken.UI {
    export class Page {
        private _content: any;

        constructor(public url: string) { }

        load(): void {
            $.get(this.url, (html) => {
                this._content = $(html);
                $('#page-container').empty().append(this._content);
                this.setup();
            });
        }

        getContent(): any {
            return this._content;
        }

        setup(): void {
        }
    }
}