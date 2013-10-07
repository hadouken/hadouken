module Hadouken.UI {
    export class Page {
        content: any;

        constructor(public url: string, public routes: Array<string>) {
        }

        init(...args: any[]): void {
            $.get(this.url, (html) => {
                this.content = $(html);

                $('#page-container').empty().append(this.content);

                if (args !== null && args.length >= 1 && typeof args[0] !== "undefined") {
                    this.load(args[0]);
                }
                else {
                    this.load();
                }
            });
        }

        load(...args: any[]): void {
        }
    }
}