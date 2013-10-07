module Hadouken.UI {
    export class Page {
        private _content: any;

        constructor(public url: string, public routes: Array<string>) {
        }

        init(): void {
            $.get(this.url, (html) => {
                this._content = $(html);
                $('#page-container').empty().append(this._content);
                this.load();
            });
        }

        getContent(): any {
            return this._content;
        }

        load(): void {
        }
    }
}