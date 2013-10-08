module Hadouken.UI {
    export class Dialog {
        private _content: any;

        constructor(public url: string) { }

        show(): void {
            $.get(this.url, (html) => {
                console.log('get get get');

                this._content = $(html);
                this._content.modal();
                this._content.on('hidden.bs.modal', function () {
                    $(this).remove();
                });

                this.onShow();
            });
        }

        close(): void {
            this._content.modal('hide');
        }

        getContent(): any {
            return this._content;
        }

        onShow(): void {
        }
    }
}