module Hadouken.UI {
    export class Page {
        public content: string;

        constructor(public url: string) { }

        load(): void {
            $.get(this.url, (html) => {
                this.content = $(html);
                $('#page-container').empty().append(this.content);
                this.setup();
            });
        }

        setup(): void {
        }
    }
}