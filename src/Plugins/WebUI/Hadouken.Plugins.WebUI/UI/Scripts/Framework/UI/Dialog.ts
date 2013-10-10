module Hadouken.UI {
    export class Dialog {
        private _content: any;

        constructor(public url: string) { }

        show(): void {
            $.get(this.url, (html) => {
                this._content = $(html);
                this._content.modal();

                var that = this;

                this._content.on('hidden.bs.modal', function () {
                    $(this).remove();
                    that.onClosed();
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

        onClosed(): void {
        }
    }
}