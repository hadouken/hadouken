module Hadouken.UI {
    export class Page {
        private _content: any;

        constructor(public url: string, public routes: Array<string>) {
        }

        init(...args: any[]): void {
            $.get(this.url, (html) => {
                this._content = $(html);
                $('#page-container').empty().append(this._content);

                if (args !== null && args.length >= 1) {
                    this.load(args[0]);
                }
                else {
                    this.load();
                }
            });
        }

        getContent(): any {
            return this._content;
        }

        load(...args: any[]): void {
        }
    }
}